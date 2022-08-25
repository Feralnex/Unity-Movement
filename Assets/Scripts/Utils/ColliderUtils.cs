using UnityEngine;

static class ColliderUtils
{
    public static Vector3[] GetBoundingBoxCorners(this Collider collider)
    {
        Vector3 extents = collider.bounds.size / 2;
        Vector3[] corners = new Vector3[]
        {
            new Vector3(extents[0], extents[1], extents[2]),
            new Vector3(extents[0], extents[1], extents[2]) * -1,
            new Vector3(-extents[0], extents[1], extents[2]),
            new Vector3(-extents[0], extents[1], extents[2]) * -1,
            new Vector3(extents[0], -extents[1], extents[2]),
            new Vector3(extents[0], -extents[1], extents[2]) * -1,
            new Vector3(extents[0], extents[1], -extents[2]),
            new Vector3(extents[0], extents[1], -extents[2]) * -1,
        };

        return corners;
    }
}
