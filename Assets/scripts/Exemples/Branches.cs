using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Branches : MonoBehaviour
{
    [SerializeField]
    private GameObject p1;
    [SerializeField]
    private GameObject p2;
    [SerializeField]
    private GameObject p3;
    [SerializeField]
    private int force = 10;
    void Start()
    {
        Vector3 direction = new Vector3(10,10,0);
        List<Vector3> points = new List<Vector3>();
        points.Add(p1.transform.position);
        points.Add(p2.transform.position);
        points.Add(p3.transform.position);
        CreateBranchFromPoint(points, p1.transform.position, direction);
    }

    Vector3 vectFrom2Points(Vector3 point1, Vector3 point2)
    {
        return new Vector3(point2.x - point1.x, point2.y - point1.y, point2.z - point1.z);
    }

    //Orthogonal vertical triangle rouge.
    Vector3 orthogonal1(Vector3 vect)
    {
        return new Vector3(0,-vect.z, vect.y);
    }
    //Orthogonal vertical triangle bleu.
    Vector3 orthogonal2(Vector3 vect)
    {
        return new Vector3(-vect.y, vect.x, 0);
    }
    //Orthogonal horizontal triangle rouge.
    Vector3 orthogonal3(Vector3 vect)
    {
        return new Vector3(0,-vect.z, vect.x);
    }
    //Orthogonal horizontal triangle bleu.
    Vector3 orthogonal4(Vector3 vect)
    {
        return new Vector3(-vect.y, vect.x, 0);
    }

    //Orthogonal profondeur triangle rouge.
    Vector3 orthogonal5(Vector3 vect)
    {
        return new Vector3(vect.z,0, 0);
    }
    //Orthogonal profondeur triangle bleu.
    Vector3 orthogonal6(Vector3 vect)
    {
        return new Vector3(-vect.y, vect.z, 0);
    }


    List<Vector3> champsDeVision(Vector3 direction,Vector3 p1,int choix)
    {
        Vector3 meridianne=p1+direction;
        Vector3 orthogo;
        Vector3 othogo2;
        switch (choix){
            case 0://Y
                orthogo = orthogonal1(direction);
                othogo2 = orthogonal2(direction);
                break;
            case 1://X
                orthogo = orthogonal3(direction);
                othogo2 = orthogonal4(direction);
                break;
            case 2://Z
                orthogo = orthogonal5(direction);
                othogo2 = orthogonal6(direction);
                break;
            default:
                orthogo = orthogonal1(direction);
                othogo2 = orthogonal2(direction);
                break;
        }
        Vector3 p2 = meridianne + orthogo * 1.5f;
        Vector3 p3 = meridianne - orthogo * 1.5f;
        Vector3 p4 = meridianne + othogo2 * 1.5f;
        Vector3 p5 = meridianne - othogo2 * 1.5f;
        //dessiner un trait de p1 à p2 et de p1 à p3 et de p2 à p3
        //Debug.Log("p1 : "+p1);
        //Debug.Log("p2 : "+p2);
        //Debug.Log("p3 : "+p3);
        //Debug.Log("p4 : "+p4);
        //Debug.Log("p5 : "+p5);
        //Debug.DrawLine(p1, p2, Color.red, 1000f);
        //Debug.DrawLine(p1, p3, Color.red, 1000f);
        //Debug.DrawLine(p2, p3, Color.red, 1000f);
        //Debug.DrawLine(p1, p4, Color.blue, 1000f);
        //Debug.DrawLine(p1, p5, Color.blue, 1000f);
        //Debug.DrawLine(p4, p5, Color.blue, 1000f);
        return new List<Vector3> { p1, p2, p3, p4, p5 };
    }


    bool triangleDesEnfers(Vector3 a,Vector3 b, Vector3 p)
    //https://www.youtube.com/watch?v=kkucCUlyIUE&ab_channel=Quantale
    /*
    On va appliqué E a toute les couples de dimensions possible et pas seulement x,y car nous somme en 3D
    E(a,b) = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x)
    E(a,b) > 0 si b est à gauche de a
    E(a,b) < 0 si b est à droite de a
    E(a,b) = 0 si b est sur a
    */
    {

       return (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x)>=0  ||
         (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x) <= 0;
    }

    bool estEnVu(List<Vector3> points, Vector3 p)
    {
        return triangleDesEnfers(points[0], points[1], p) && triangleDesEnfers(points[2], points[1], p) && triangleDesEnfers(points[1], points[2], p);
    }

    bool DansPyramideY(List<Vector3> pyramide, Vector3 p)
    {
        float coeffDirecteur = (pyramide[3].y - pyramide[0].y) / (pyramide[3].x - pyramide[0].x);
        coeffDirecteur = Mathf.Abs(coeffDirecteur);
        //square of p.x^2 + p.z^2
        float xz = Mathf.Sqrt(Mathf.Pow(p.x, 2) + Mathf.Pow(p.z, 2));
        //Debug.Log(coeffDirecteur);
        //Debug.Log(xz);
        float ypoint = coeffDirecteur * xz;
        //Debug.Log(ypoint);
        //Debug.Log(pyramide[1].y);
        return p.y >= ypoint && ypoint <= pyramide[1].y && p.y <= pyramide[1].y;
    }

    bool DansPyramideX(List<Vector3> pyramide, Vector3 p)
    {
        float coeffDirecteur = (pyramide[3].x - pyramide[0].x) / (pyramide[3].y - pyramide[0].y);
        coeffDirecteur = Mathf.Abs(coeffDirecteur);
        //square of p.x^2 + p.z^2
        float yz = Mathf.Sqrt(Mathf.Pow(p.y, 2) + Mathf.Pow(p.z, 2));
        //Debug.Log(coeffDirecteur);
        //Debug.Log(yz);
        float xpoint = coeffDirecteur * yz;
        //Debug.Log(xpoint);
        //Debug.Log(pyramide[1].x);
        return p.x >= xpoint && xpoint <= pyramide[1].x && p.x <= pyramide[1].x;
    }

    bool DansPyramideZ(List<Vector3> pyramide, Vector3 p)
    {
        float coeffDirecteur = (pyramide[3].y - pyramide[0].y) / (pyramide[3].z - pyramide[0].z);
        coeffDirecteur = Mathf.Abs(coeffDirecteur);
        //square of p.x^2 + p.z^2
        float xy = Mathf.Sqrt(Mathf.Pow(p.x, 2) + Mathf.Pow(p.y, 2));
        //Debug.Log(coeffDirecteur);
        //Debug.Log(xy);
        float zpoint = coeffDirecteur * xy;
        //Debug.Log(zpoint);
        //Debug.Log(pyramide[1].z);
        return p.z >= zpoint && zpoint <= pyramide[1].z && p.z <= pyramide[1].z;
    }


    float distanceDe(Vector3 v1 ,Vector3 v2)
    {
       return Mathf.Abs(v1.x-v2.x)+Mathf.Abs(v1.y-v2.y)+Mathf.Abs(v1.z-v2.z);
    }

    int MatchingVector(Vector3 direction)
    {
        List <Vector3> maListe = new List<Vector3>();
        maListe.Add(new Vector3(0,0,1)); //0 Devant
        maListe.Add(new Vector3(0,0,-1)); //1 Derriere
        maListe.Add(new Vector3(0,1,0));//2 Haut
        maListe.Add(new Vector3(1,0,0));//3 Droite
        maListe.Add(new Vector3(-1,0,0));//4 Gauche
        maListe.Add(new Vector3(-1,0,-1));//5 Gauche-Derriere
        maListe.Add(new Vector3(-1,0,1));//6 Gauche-Devant
        maListe.Add(new Vector3(1,0,-1));//7 Droite-Derriere
        maListe.Add(new Vector3(1,0,1));//8 Droite-Devant
        maListe.Add(new Vector3(1,1,0));//9 Haut-Droite
        maListe.Add(new Vector3(-1,1,0));//10 Haut-Gauche
        maListe.Add(new Vector3(0,1,1));//11 Haut-Devant
        maListe.Add(new Vector3(0,1,-1));//12 Haut-Derriere
        maListe.Add(new Vector3(-1,1,-1));//13 Haut-Gauche-Derriere
        maListe.Add(new Vector3(-1,1,1));//14 Haut-Gauche-Devant
        maListe.Add(new Vector3(1,1,-1));//15 Haut-Droite-Derriere
        maListe.Add(new Vector3(1,1,1));//16 Haut-Droite-Devant

        int index = 0;
        float minimum = distanceDe(maListe[0],direction);
        for(int i = 1; i<maListe.Count; i++)
        {
            float distance = distanceDe(maListe[i],direction);
            if(distance <= minimum)
            {
                minimum = distance;
                index = i;
            }
        }
        return index;
    }

    void CreateBranchFromPoint(List<Vector3> allPoints, Vector3 p, Vector3 direction)
    {//Y X Z
        List<Vector3> pointsVu = new List<Vector3>();
        List<Vector3> visu1 = new List<Vector3>();
        List<Vector3> visu2 = new List<Vector3>();
        allPoints.Remove(p);
        Vector3 vecteurNormal;
        Vector3 vecteurNormal2;
        bool isVertical = false;
        bool isHorizontal = false;
        bool isProfondeur = false;
        switch (MatchingVector(direction))
        {
            case 0:
                vecteurNormal = new Vector3(0,0,force);
                visu1 = champsDeVision(vecteurNormal, p,2);
                isProfondeur = true;
                break;
            case 1:
                vecteurNormal = new Vector3(0,0,-force);
                visu1 = champsDeVision(vecteurNormal, p,2);
                isProfondeur = true;
                break;
            case 2:
                vecteurNormal = new Vector3(0,force,0);
                visu1 = champsDeVision(vecteurNormal, p,0);
                isVertical = true;
                break;
            case 3:
                vecteurNormal = new Vector3(force,0,0);
                visu1 = champsDeVision(vecteurNormal, p,1);
                isHorizontal = true;
                break;
            case 4:
                vecteurNormal = new Vector3(-force,0,0);
                visu1 = champsDeVision(vecteurNormal, p,1);
                isHorizontal = true;
                break;
            case 5:
                vecteurNormal = new Vector3(-force,0,0);
                vecteurNormal2 = new Vector3(0,0,-force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
            case 6:
                vecteurNormal = new Vector3(-force,0,0);
                vecteurNormal2 = new Vector3(0,0,force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
            case 7:
                vecteurNormal = new Vector3(force,0,0);
                vecteurNormal2 = new Vector3(0,0,-force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
            case 8:
                vecteurNormal = new Vector3(force,0,0);
                vecteurNormal2 = new Vector3(0,0,force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
            case 9:
                vecteurNormal = new Vector3(force,0,0);
                vecteurNormal2 = new Vector3(0,force,0);
                visu1 = champsDeVision(vecteurNormal2,p,0);
                visu2 = champsDeVision(vecteurNormal, p,1);
                isVertical = true;
                isHorizontal = true;
                break;
            case 10:
                vecteurNormal = new Vector3(0,force,0);
                vecteurNormal2 = new Vector3(-force,0,0);
                visu1 = champsDeVision(vecteurNormal, p,0);
                visu2 = champsDeVision(vecteurNormal2,p,1);
                isVertical = true;
                isHorizontal = true;
                break;
            case 11:
                vecteurNormal = new Vector3(0,force,0);
                vecteurNormal2 = new Vector3(0,0,force);
                visu1 = champsDeVision(vecteurNormal, p,0);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isVertical = true;
                break;
            case 12:
                vecteurNormal = new Vector3(0,force,0);
                vecteurNormal2 = new Vector3(0,0,-force);
                visu1 = champsDeVision(vecteurNormal, p,0);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isVertical = true;
                break;
            case 13:
                vecteurNormal = new Vector3(-force,0,0);
                vecteurNormal2 = new Vector3(0,0,-force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
            case 14:
                vecteurNormal = new Vector3(-force,0,0);
                vecteurNormal2 = new Vector3(0,0,force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
            case 15:
                vecteurNormal = new Vector3(force,0,0);
                vecteurNormal2 = new Vector3(0,0,-force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
            case 16:
                vecteurNormal = new Vector3(force,0,0);
                vecteurNormal2 = new Vector3(0,0,force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
            default:
                vecteurNormal = new Vector3(force,0,0);
                vecteurNormal2 = new Vector3(0,0,force);
                visu1 = champsDeVision(vecteurNormal, p,1);
                visu2 = champsDeVision(vecteurNormal2,p,2);
                isProfondeur = true;
                isHorizontal = true;
                break;
        }
        Debug.Log("isVertical: "+isVertical);
        Debug.Log("isHorizontal : "+isHorizontal);
        Debug.Log("isProfondeur : "+isProfondeur);
        if(isVertical)
        {
            foreach (Vector3 point in allPoints)
            {
                if (DansPyramideY(visu1, point))
                {
                    pointsVu.Add(point);
                }
            }
            if(isHorizontal)
            {
                List<Vector3> pointsVuCpy = new List<Vector3>(pointsVu);
                foreach(Vector3 elem in pointsVu)
                {
                    if(!DansPyramideX(visu2,elem))
                    {
                        pointsVuCpy.Remove(elem);
                    }
                }
                pointsVu = pointsVuCpy;
            }else if(isProfondeur)
            {
                List<Vector3> pointsVuCpy = new List<Vector3>(pointsVu);
                foreach(Vector3 elem in pointsVu)
                {
                    if(!DansPyramideZ(visu2,elem))
                    {
                        pointsVuCpy.Remove(elem);
                    }
                }
                pointsVu = pointsVuCpy;
            }
        }else if(isHorizontal)
        {
            foreach (Vector3 point in allPoints)
            {
                if (DansPyramideX(visu1, point))
                {
                    pointsVu.Add(point);
                }
            }
            if(isVertical)
            {
                List<Vector3> pointsVuCpy = new List<Vector3>(pointsVu);
                foreach(Vector3 elem in pointsVu)
                {
                    if(!DansPyramideY(visu2,elem))
                    {
                        pointsVuCpy.Remove(elem);
                    }
                }
                pointsVu = pointsVuCpy;
            }else if(isProfondeur)
            {
                List<Vector3> pointsVuCpy = new List<Vector3>(pointsVu);
                foreach(Vector3 elem in pointsVu)
                {
                    if(!DansPyramideZ(visu2,elem))
                    {
                        pointsVuCpy.Remove(elem);
                    }
                }
                pointsVu = pointsVuCpy;
            }
        }else if(isProfondeur)
        {
            foreach (Vector3 point in allPoints)
            {
                if (DansPyramideZ(visu1, point))
                {
                    pointsVu.Add(point);
                }
            }

            if(isVertical)
            {
                List<Vector3> pointsVuCpy = new List<Vector3>(pointsVu);
                foreach(Vector3 elem in pointsVu)
                {
                    if(!DansPyramideY(visu2,elem))
                    {
                        pointsVuCpy.Remove(elem);
                    }
                }
                pointsVu = pointsVuCpy;
            }else if(isHorizontal)
            {
                List<Vector3> pointsVuCpy = new List<Vector3>(pointsVu);
                foreach(Vector3 elem in pointsVu)
                {
                    if(!DansPyramideX(visu2,elem))
                    {
                        pointsVuCpy.Remove(elem);
                    }
                }
                pointsVu = pointsVuCpy;
            }
        }
        
        
        Debug.Log(pointsVu.Count);
        if(pointsVu.Count >= 1)
        {
            Debug.DrawLine(pointsVu[0], p, Color.green, 1000f);
            Vector3 direction2 = pointsVu[0] - p;
            CreateBranchFromPoint(allPoints, pointsVu[0], direction2);
        }
    }

}