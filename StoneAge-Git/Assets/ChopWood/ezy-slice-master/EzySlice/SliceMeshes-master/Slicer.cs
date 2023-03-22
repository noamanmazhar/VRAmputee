using UnityEngine;
using EzySlice;
public class Slicer : MonoBehaviour
{
    public Material materialAfterSlice;
    public LayerMask sliceMask;
    public bool isTouched;

    private void Update()
    {
        if (isTouched == true)
        {
            isTouched = false;

            Collider[] objectsToBeSliced = Physics.OverlapBox(transform.position, new Vector3(1, 0.1f, 0.1f), transform.rotation, sliceMask);
            
            foreach (Collider objectToBeSliced in objectsToBeSliced)
            {
                SlicedHull slicedObject = SliceObject(objectToBeSliced.gameObject, materialAfterSlice);

                GameObject upperHullGameobject = slicedObject.CreateUpperHull(objectToBeSliced.gameObject, materialAfterSlice);
                GameObject lowerHullGameobject = slicedObject.CreateLowerHull(objectToBeSliced.gameObject, materialAfterSlice);

                upperHullGameobject.transform.position = objectToBeSliced.transform.position;
                lowerHullGameobject.transform.position = objectToBeSliced.transform.position;

                MakeItPhysical(upperHullGameobject);
                MakeItPhysical(lowerHullGameobject);


    //lowerHullGameobject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                Destroy(objectToBeSliced.gameObject);
                // Add point in score manager script
                ScoreManagerWood.instance.AddPoint();
            }
        }
    }

    private void MakeItPhysical(GameObject obj)
    {
        
        obj.AddComponent<MeshCollider>().convex = true;
        obj.AddComponent<Rigidbody>();
        obj.GetComponent<Rigidbody>().mass = 9.0f;
        //obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        //if (obj.transform.position.y < 1.7f)
       // {
            //obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
      //  }

    }

    private SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        return obj.Slice(transform.position, transform.up, crossSectionMaterial);
    }


}
