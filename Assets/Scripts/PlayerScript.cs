using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private enum Movement
    {
        Undefined,
        IsIdle,
        IsWalking,
        IsAtGoal,
        //IsAvoiding,
        //IsDying,
        //IsDancing

    }
        

    public float movementSpeed;
    
    private Vector3 PlayerObjectPosition=Vector3.zero;
    private Vector3 WhereMouseIsClicked = Vector3.zero;
    private Vector3 StartPosition = Vector3.zero;
    private Vector3 CurrentPosition = Vector3.zero;
    //private Movement MovementType = Movement.Undefined;


    // Start is called before the first frame update
    void Start()
    {
        StartPosition= transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentPosition = transform.position;
        
        if (Input.GetMouseButtonDown(0))
        {
            WhereMouseIsClicked = GetWhereMouseIsClicked();
        }

        MoveToMouse(WhereMouseIsClicked);
        Movement CurrentMove = GetMovementType(WhereMouseIsClicked);
        
        PlayAnimation(CurrentMove);

    }

    /// <summary>
    /// Returnerer en vector, der angiver hvor musen bliver klikket
    /// på samme flade, som objectet (skelettet) bevæger sig på.
    /// </summary>
    /// <returns>
    /// Returner en Vector3, der angiver hvor vi klikker.
    /// Eller - punktet (0,0,0) svarende til Vector3.zero
    /// </returns>
    private Vector3 GetWhereMouseIsClicked()
    {
        //Sæt en begynderværdi, så vi altid kan checke at vi faktisk registrerer WhereIsMouse  
        Vector3 WhereIsMouse=Vector3.zero;
        

        Plane PG = new Plane(Vector3.up, CurrentPosition);
        //Den flade som selve RayCast kan "bevæge sig på, beregnet udfra CurrentPosition

        float DistanceFromCamera = 0.0f;
        //Det er en såkaldt outputbuffer. Bemærk det lille "out" i parameteren i function PG.Raycast længere nede i koden.
        //Tallet bliver beregnet som afstanden på fladen PG, 
        //mellem kameraet og der hvor man klikker med musen. I virkeligheden temmelig kompliceret.
        //(Jeg skal gerne forklare).

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Her defineres et ray mellem objectet (skelettet) og der hvor i klikker (Input.mousePosition).

        if (PG.Raycast(ray, out DistanceFromCamera))  //Hvis distancen er forskellig fra nul, da
        {
            WhereIsMouse = ray.GetPoint(DistanceFromCamera);
        }

        //Hvis vi IKKE kunne beregne hvor der blev klikket da er WhereIsMouse=begynderværdien (Vector3.zero) 
        return WhereIsMouse;
         
    }

    /// <summary>
    /// Vender Objectet (Skelettet) mod det punkt hvor man klikker, og bevæger 
    /// Objectet (Skelettet) henimod punktet, indtil punktet bliver nået. 
    /// </summary>
    /// <param name="pWhereMouseIsClicked">
    /// Der, hvor man klikker.
    /// </param>
    private void MoveToMouse(Vector3 pWhereMouseIsClicked)
    {
        Debug.DrawLine(CurrentPosition, pWhereMouseIsClicked, Color.red, 2.5f);
        transform.LookAt(pWhereMouseIsClicked);
        
        Vector3 NextPosition= Vector3.MoveTowards(CurrentPosition, pWhereMouseIsClicked, movementSpeed); //Gå til hvor vi klikkede 
        //Den nye position

        transform.position = NextPosition;
        //flyt Objectet (Skelettet), til den nye position
    }

    /// <summary>
    /// Finder ud af hvad Objectet (Skelettet), er igang med
    /// </summary>
    /// <param name="pWhereMouseIsClicked">
    /// Der hvor vi klikkede (pWhereMouseIsClicked) 
    /// </param>
    /// <returns>Et Movement, baseret på hvor objectet (Skelettet) er, 
    /// i forhold til der hvor vi klikkede (pWhereMouseIsClicked) 
    /// </returns>
    private Movement GetMovementType(Vector3 pWhereMouseIsClicked)
    {
        //Sæt returnval til at være udefineret, således at vi senere
        //kan checke om returnval overhoved bliver givet en værdi

        Movement returnval=Movement.Undefined;
        
        //Er vi overhovedet startet
        if (CurrentPosition==StartPosition) 
        {
            returnval= Movement.IsIdle;
        }
        
        //Er vi på vej til målet
        if (CurrentPosition != pWhereMouseIsClicked)
        {
            returnval= Movement.IsWalking;
        }

        //Når programmet starter, registrerer Unity automatisk et "begynderklik",
        //selvom vi IKKE har klikket nogen steder!!
        //Derfor skal vi checked for at vi IKKE er i StartPosition 
        //OG om vi nu ved målet (der hvor vi SELV klikkede)
        
        if ( (CurrentPosition != StartPosition) && (CurrentPosition == pWhereMouseIsClicked) )
        {
            returnval=Movement.IsAtGoal;
        }
             
        //Hvis vi IKKE kunne bestemme et Movement, da returnerer vi begynderværdien (Movement.Undefined)
        return returnval;
    }
    
    
    /// <summary>
    /// Afspiller en animation, udfra det som objectet foretager sig (Angivet i parameteren pMovement).
    /// Hvis pMovement=Movement.Undefined skriver vi et eller andet til loggen.
    /// </summary>
    /// <param name="pMovementType">
    /// Det, objectet foretager sig. 
    /// </param>
    private void PlayAnimation(Movement pCurrentMove)
    {
        Animation Anim = GetComponent<Animation>();
        switch (pCurrentMove)
        {
            case Movement.IsIdle:
                Anim.CrossFade("DS_onehand_idle_A");
                break;
            
            case Movement.IsWalking:
                Anim.CrossFade("DS_onehand_walk");
                break;
            
            case Movement.IsAtGoal:
                //Når objectet (Skellet) har nået sit mål, da afspil "Attack-animationenen"
                Anim.CrossFade("DS_onehand_attack_A");
                break;
            
            case Movement.Undefined:    //Også kaldet default !?
                Debug.Log("PlayerScript - PlayAnimation: " + "Skid i havet og hop i havnen");
                break;


        }
    }

    
    
}
