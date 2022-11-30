using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamic_Leg_Controller : MonoBehaviour
{
    public GameObject outLegBase;
    Transform LegOuter;

    public Transform Body;

    public AnimationCurve yCurve;

    private Vector3 direction = Vector3.down;
    private Vector3 kneePos;

    private Vector3 footPos;
    private Vector3 targetPosition;
    private Vector3 lastTargetPosition;
    private Vector3 legPlaneNormal;
    private Vector3 hitPos;
    private Vector3 rotAxis;
    private Vector3 relectedDir;



    public float LegLength = 1f;

    public float distMod = 1;
    public float DirMod = 1;

    private float maxDistance = 1f;
    public float minDistance = 0.1f;

    public float maxHipAngle = 30;

    public float duration = 1;

    public float hipAngle = 0;

    private int hipDirection = 1;

    private float t = 0;

    public bool isMoving = false;

    void Start()
    {
        LegOuter = GameObject.Instantiate(outLegBase).transform;
        //turn off collider
        LegOuter.GetComponent<Collider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        maxDistance = LegLength * 2;

        CastRay();

        hipAngle = SolveHipAngle();

        Debug.DrawLine(hitPos, transform.position, Color.red);

        Debug.DrawRay(kneePos, rotAxis, Color.green);

        //check distance
        if(!isMoving && CheckInvalidLegPos()){
            targetPosition = hitPos;

            //trigger animation
            StartLegAnimation();
        }

        UpdateLegAnimation();
        
    }

    bool CheckInvalidLegPos(){
        float dist = Vector3.Distance(transform.position, footPos);
        if(dist > maxDistance * distMod){
            return true;
        }
        if(dist < minDistance){
            return true;
        }
        if(hipAngle > maxHipAngle){

            return true;
        }
        return false;
    }

    //cast a ray from the leg to the ground
    void CastRay()
    {
        direction = Vector3.ProjectOnPlane(transform.position - Body.position, transform.up);

        direction +=  Vector3.down * DirMod;

        hipAngle = Mathf.Clamp(hipAngle, 0 , maxHipAngle-1);

        direction = Quaternion.AngleAxis(hipAngle,-rotAxis) * direction;

        direction.Normalize();

        Debug.DrawRay(transform.position, direction * maxDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
        {
            hitPos = hit.point;
        }
    }

    void StartLegAnimation(){
        //trigger animation
        t = 0;
        isMoving = true;
        lastTargetPosition = footPos;
    }

    void UpdateLegAnimation(){

        if(isMoving){

            t += Time.deltaTime;
            if(t > duration){
                t = duration;
                isMoving = false;
            }
            
            Vector3 lerpPos = Vector3.Lerp(lastTargetPosition, targetPosition, t/duration);

            float yPos = yCurve.Evaluate(t/duration);

            footPos = new Vector3(lerpPos.x, yPos, lerpPos.z);
        }

        legPlaneNormal = SolveLegPlaneNormal();
        kneePos = SolveKneePos();
        
        Debug.DrawLine(transform.position, kneePos, Color.green);
        Debug.DrawLine(kneePos, footPos, Color.blue);

        LegOuter.position = kneePos;
        LegOuter.LookAt(footPos, Vector3.Cross(legPlaneNormal, hitPos - Body.position));

    }

    Vector3 SolveKneePos(){
        float d = Vector3.Distance(footPos, transform.position);
        //get angle from adject side and hypotenuse
        float a = Mathf.Acos((d/2) / LegLength) * Mathf.Rad2Deg;
        Vector3 v = footPos - transform.position;
        v.Normalize();
        //rotate v by a
        v = Quaternion.AngleAxis(a, legPlaneNormal) * v;
        return transform.position + v * LegLength;
        //return transform.position + (footPos - transform.position)/2;
    }

    Vector3 SolveLegPlaneNormal(){
        return Vector3.Cross(footPos - Body.position , transform.up).normalized;
    }

    float SolveHipAngle(){
        Vector3 projectedToBody = Vector3.ProjectOnPlane(transform.position - Body.position, transform.up);
        Vector3 projectedToFoot = Vector3.ProjectOnPlane(footPos - transform.position, transform.up);

        rotAxis = Vector3.Cross(
        projectedToBody
        , projectedToFoot).normalized;

        rotAxis = Vector3.Project(rotAxis, transform.up);

        return Vector3.Angle(projectedToBody, projectedToFoot);
    }
}
