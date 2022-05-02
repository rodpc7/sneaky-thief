using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RotatingGuard : MonoBehaviour
{
    // Start is called before the first frame update
    public static event System.Action OnRotGuardHasSpottedPlayer;
    public Transform pathHolder;
    public float speed = 2;
    public float waitTime = .3f;
    public float turnSpeed = 90; //(90 graus por segundo)
    public float timeToSpotPlayer = .5f;//Tempo que o player tem para sair do raio de dete??o
    float playerVisibleTimer;

    public Light spotlight;
    public float viewDistance;
    float viewAngle;
    public LayerMask viewMask;
    Color OriginalSpotlightcolor;

    Transform player;

    //Movimento do Guarda
    void Start()
    {
        //Guard.OnGuardHasSpottedPlayer();
        player = GameObject.FindGameObjectWithTag("Player").transform;//Vai buscar o jogador
        viewAngle = spotlight.spotAngle;//Recebe o angulo do spotlight
        OriginalSpotlightcolor = spotlight.color;//Guarda a cor
        Vector3[] waypoints = new Vector3[pathHolder.childCount];//Array de todas a posi??es
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;//Recebe todas as posi??es
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);//mantem as coordenadas do y, apenas atualiza o x e z para o waypoint
        }

        StartCoroutine(FollowPath(waypoints));//Inicia a Rotina
    }
    private void Update()
    {
        if (CanSeePlayer())
        {
            //Debug.Log("Player detetado");
            playerVisibleTimer += Time.deltaTime;//soma o deltaTime
            //spotlight.color = Color.red;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;//subtrai
            //spotlight.color = OriginalSpotlightcolor;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);//Restringe o valor entre o max e o minimo
        spotlight.color = Color.Lerp(OriginalSpotlightcolor, Color.red, playerVisibleTimer / timeToSpotPlayer);//smooth faz o valor interpolado 

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
           if (OnRotGuardHasSpottedPlayer != null)
            {
                Debug.Log("Player detetado");
                OnRotGuardHasSpottedPlayer();//ativa o evento caso na seja null
                StopAllCoroutines();
            }
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)//Verifica se a distancia entre o guarda e o player ? menor que a viewdistance, para otimiza podemos faze o calculo pela raiz
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)//verifica se a diferen?a entre os angulos ? inferior ao viewangle/2
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))//criamos um raio entre o guarda e o player e sabemos se algo intreseta ent existe um obstacle
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)//Faz a itera??o
    {
        //transform.position = waypoints[0];//Coloca o guarda no primeiro ponto
        int targetWaypointIndex = 0;//Passa o index para 1
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];//Guarda o Waypoint seguinte
        transform.LookAt(targetWaypoint);

        while (true)//Loop Infinito
        {
            //transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);//Move-se do Waypoint atual para o waypoint seguinte
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;//incrementa o index e volta a zero
                targetWaypoint = waypoints[targetWaypointIndex];// Atualiza o prx waypoint
                yield return new WaitForSeconds(waitTime);//Faz um compasso de espera no ponto e passa para a itera??o seguinte
                yield return StartCoroutine(TurnToFace(targetWaypoint));//Espera que o guarda fa?a a rota??o
                //StartCoroutine(TurnToFace(targetWaypoint));//Espera que o guarda fa?a a rota??o
            
            yield return null;//corre a mesma itera??o novamente
        }


    }

    IEnumerator TurnToFace(Vector3 lookTarget)//Recebe o proximo waypoint 
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized; //calculo da dire??o
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;//calculo do angulo

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) // Tem de ser o abs pois os angulos podem ser negativos(contra relogio)/ >0.05f devido a precisao nem sempre da 0
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }


    //Desenha uma esfera em cada WayPoint
    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform WayPoint in pathHolder)//Corre todos os WayPoints no path
        {
            Gizmos.DrawSphere(WayPoint.position, .3f);//Desenha a esfera
            Gizmos.DrawLine(previousPosition, WayPoint.position);//Desenha a linha
            previousPosition = WayPoint.position;//Guarda o WayPoint Anterior
        }
        Gizmos.DrawLine(previousPosition, startPosition);//Fecha o loop

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);//Linha vermelha que representa a viewdistance do guarda
    }
}

