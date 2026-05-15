using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class KnifeBlockObject : MonoBehaviour
{
    public List<PlacePoint> placePoints = new();
    public KnifeObject[] allKnifes;
    public Collider objectCollider;

    void Awake()
    {

    }

    public PlacePoint GetAvaiablePlacePoint()
    {
        return placePoints.Find(T => T.isEmpty);
    }
}
