namespace DangQuocHieu.GameKit
{
    using UnityEngine;

    public interface IPooledObject
    {
        public void OnObjectSpawn();
        public void OnObjectDespawn();
    }

}