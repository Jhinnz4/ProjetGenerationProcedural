using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateurDeForme : MonoBehaviour
{
    public float tailleTronc = 1f;
    public float epaisseurTronc = 0.5f;
    public float hauteurArbre = 5f;

    public float epaisseurArbre = 0.5f;

    //slider from 0.1 to 1
    [Range(0.1f, 0.90f)]
    public float precisionCouronne = 0.1f;
    [Range(0.1f, 0.90f)]
    public float precisionTronc = 0.1f;

    public bool autoUpdate;

    void Start()
    {
        GenererArbre();
    }

    public void GenererArbre()
    {
        /*
            GameObject tronc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tronc.transform.localScale = new Vector3(epaisseurTronc, tailleTronc, epaisseurTronc);
            tronc.transform.position = new Vector3(0, 0, 0);

            GameObject couronne = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            couronne.transform.localScale = new Vector3(epaisseurArbre, hauteurArbre, epaisseurArbre);
            couronne.transform.position = new Vector3(0, tailleTronc + hauteurArbre / 2, 0);
            couronne.transform.parent = tronc.transform;

            TransformFormToPoint(couronne);
        */
        //select object tronc and couronne do delete them before creating new ones
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Tree");
        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }

        GameObject tronc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tronc.tag = "Tree";
        tronc.transform.position = new Vector3(0, tailleTronc, 0);

        GameObject couronne = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        couronne.tag = "Tree";
        couronne.transform.position = new Vector3(0, tailleTronc*2 + (hauteurArbre/2)-0, 0);
        couronne.transform.parent = tronc.transform;
        TransformFormToPoint(couronne, epaisseurArbre, hauteurArbre, precisionCouronne);
        TransformTroncToPoint(tronc, tailleTronc, epaisseurTronc, precisionTronc);
    }

    bool RandomBool()
    {
        return Random.Range(0, 5) == 0;
    }
    List<Vector3> pointsArray = new List<Vector3>();
    List<Vector3> pointsArray2 = new List<Vector3>();
    void TransformTroncToPoint(GameObject forme, float tailleTronc, float epaisseurTronc,float precision)
    {
        pointsArray2.Clear();
        precision = 1 - precision;
        MeshFilter meshFilter = forme.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        GameObject parent = new GameObject("points");
        parent.tag = "Tree";
        Vector3[] vertices = mesh.vertices;
        float tailleTronc2 = this.tailleTronc;
        float epaisseurTronc2 = this.epaisseurTronc;
        Vector3[] vertices2 = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 point = vertices[i];
            point.x *= epaisseurTronc;
            point.y *= tailleTronc;
            point.z *= epaisseurTronc;
            vertices2[i] = point;
        }

        //creer les points de la forme pour la visualiser par rapport au mesh et non au bounds et par rapport à hauteurArbre et epaisseurArbre
        while(tailleTronc > 0)
        {
            epaisseurTronc = epaisseurTronc2;
            while (epaisseurTronc > 0)
            {
                for (int i = 0; i < vertices2.Length; i++)
                {
                    Vector3 point = vertices[i];
                    point.x *= epaisseurTronc;
                    point.y *= tailleTronc;
                    point.z *= epaisseurTronc;
                    if (RandomBool() && RandomBool())
                    {
                        CreateVisualizationPoint(point, parent);
                        pointsArray2.Add(point);
                    }
                }
                epaisseurTronc -= precision;
            }
            tailleTronc -= precision;
        }
        
        mesh.vertices = vertices2;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        parent.transform.position = forme.transform.position;
        Bounds bounds = mesh.bounds;
        bounds.center = forme.transform.position;

        forme.GetComponent<MeshFilter>().mesh = mesh;
        mesh.bounds = bounds;
        GameObject branch = new GameObject("branch");
        branch.tag = "Tree";
        //CreateBranchFromPoint(pointsArray2, pointsArray2[0], precision, branch);
        branch.transform.position = forme.transform.position;
    }

    void TransformFormToPoint(GameObject forme, float epaisseurArbre, float hauteurArbre,float precision)
    {
        precision = 1 - precision;
        MeshFilter meshFilter = forme.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        GameObject parent = new GameObject("points");
        parent.tag = "Tree";
        Vector3[] vertices = mesh.vertices;
        float epaisseurArbre2 = this.epaisseurArbre;
        float hauteurArbre2 = this.hauteurArbre;
        Vector3[] vertices2 = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 point = vertices[i];
            point.x *= epaisseurArbre;
            point.y *= hauteurArbre;
            point.z *= epaisseurArbre;
            vertices2[i] = point;
        }
        
        //creer les points de la forme pour la visualiser par rapport au mesh et non au bounds et par rapport à hauteurArbre et epaisseurArbre
        while(epaisseurArbre > 0.2 && hauteurArbre > 0.2)
        { 
            for (int i = 0; i < vertices2.Length; i++)
            {
                Vector3 point = vertices[i];
                point.x *= epaisseurArbre;
                point.y *= hauteurArbre;
                point.z *= epaisseurArbre;
                if(RandomBool() && RandomBool())
                {
                    CreateVisualizationPoint(point, parent);
                    pointsArray.Add(point);
                }
            }
            epaisseurArbre -= precision;
            hauteurArbre -= precision;
        }
        mesh.vertices = vertices2;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        parent.transform.position = forme.transform.position;
        Bounds bounds = mesh.bounds;
        bounds.center = forme.transform.position;
        
        forme.GetComponent<MeshFilter>().mesh = mesh;
        mesh.bounds = bounds;
        //CreateBranchFromPoint(pointsArray, pointsArray[0], precision, parent);
    }

    void CreateVisualizationPoint(Vector3 position, GameObject parent)
    {
        GameObject visualizationPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualizationPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        visualizationPoint.transform.position = position;
        visualizationPoint.transform.parent = parent.transform;
    }

    void CreateBranchFromPoint(List<Vector3> points, Vector3 point, float precision, GameObject branch)
    {
        List<Vector3> pointsProches = new List<Vector3>();
        points.Remove(point);
        float desiredAngle = 45f;

        foreach (Vector3 point2 in points)
        {
            if (point2.y > point.y && point2.y < point.y + 0.7f && Vector3.Angle(point2 - point, Vector3.up) < desiredAngle)
            {
                pointsProches.Add(point2);
            }
        }

        if (pointsProches.Count >= 1)
        {
            Vector3 pointSuivant = pointsProches[0];
            Debug.Log(" [" + point.x + "," + point.y + "," + point.z + "],[" + pointSuivant.x + "," + pointSuivant.y + "," + pointSuivant.z + "]");
            
            // Calculer la hauteur du cylindre
            float hauteurCylindre = (pointSuivant.y - point.y) / 2f;

            // Créer le cylindre à partir de la base
            GameObject cylindre = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylindre.transform.localScale = new Vector3(0.1f, hauteurCylindre * 2f, 0.1f);
            
            // Calculer la position du cylindre
            Vector3 positionCylindre = new Vector3(point.x, point.y + hauteurCylindre, point.z);

            // Appliquer la rotation au cylindre
            cylindre.transform.rotation = Quaternion.LookRotation(pointSuivant - point, Vector3.up);

            // Ajuster la position du cylindre après la rotation
            cylindre.transform.position = positionCylindre;

            // Définir le parent
            cylindre.transform.parent = branch.transform;

            // Appeler récursivement pour créer la suite de la branche
            CreateBranchFromPoint(points, pointSuivant, precision, branch);
        }
        else
        {
            Debug.Log("Fin de la branche");
        }
            /*
            points.Remove(point);
            
            List<Vector3> pointsProches = new List<Vector3>();
            
            foreach (Vector3 point2 in points)
            {
                if (point2.y > point.y && point2.y < point.y + 2f)
                {
                    pointsProches.Add(point2);
                }
            }

            if (pointsProches.Count == 1)
            {
                Vector3 pointSuivant = pointsProches[0];
                points.Remove(pointSuivant);

                GameObject cylindre = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylindre.transform.parent = branch.transform;

                // Utiliser Quaternion.LookRotation pour calculer la rotation entre les deux points
                cylindre.transform.rotation = Quaternion.LookRotation(pointSuivant - point);

                cylindre.transform.localScale = new Vector3(0.1f, Vector3.Distance(point, pointSuivant) / 2, 0.1f);
                cylindre.transform.position = new Vector3(point.x, point.y + Vector3.Distance(point, pointSuivant) / 2, point.z);

                return CreateBranchFromPoint(points, pointSuivant, precision, branch);
            }
            else if (pointsProches.Count > 1)
            {
                Vector3 pointSuivant = Vector3.zero;
                
                foreach (Vector3 point2 in pointsProches)
                {
                    pointSuivant += point2;
                }
                
                foreach (Vector3 point2 in pointsProches)
                {
                    points.Remove(point2);
                }

                pointSuivant /= pointsProches.Count;
                points.Remove(pointSuivant);

                GameObject cylindre = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylindre.transform.parent = branch.transform;

                // Utiliser Quaternion.LookRotation pour calculer la rotation entre les deux points
                cylindre.transform.rotation = Quaternion.LookRotation(pointSuivant - point);

                cylindre.transform.localScale = new Vector3(0.1f, Vector3.Distance(point, pointSuivant) / 2, 0.1f);
                cylindre.transform.position = new Vector3(point.x, point.y + Vector3.Distance(point, pointSuivant) / 2, point.z);

                return CreateBranchFromPoint(points, pointSuivant, precision, branch);
            }
            else
            {
                return true;
            }
            */

    }

}
