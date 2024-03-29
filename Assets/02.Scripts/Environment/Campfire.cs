using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Campfire : MonoBehaviour
{
    public int Damage = 20;
    public float Cooltime = 1f;
    private float _timer = 0f;

    private IDamaged _target = null;

    private void OnTriggerEnter(Collider col)
    {
        IDamaged damagedObject = col.GetComponent<IDamaged>();
        if (damagedObject == null)
        {
            return;
        }

        PhotonView photonView = col.GetComponent<PhotonView>();
        if (photonView == null || !photonView.IsMine)
        {
            return;
        }

        _target = damagedObject;
    }

    private void OnTriggerStay(Collider col)
    {
        if (_target == null)
        {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= Cooltime)
        {
            _timer = 0f;
            _target.Damaged(Damage, -1);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        IDamaged damagedObject = col.GetComponent<IDamaged>();
        if (damagedObject == null)
        {
            return;
        }

        if (damagedObject == _target)
        {
            _target = null;
            _timer = 0f;
        }
    }
}