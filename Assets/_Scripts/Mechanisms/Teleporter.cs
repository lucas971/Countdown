using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Teleporter To;
    [SerializeField] private float cooldown;

    private bool inCooldown = false;

    private void OnDrawGizmos()
    {
        if (To)
            Debug.DrawLine(transform.position, To.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (inCooldown || To == null)
            return;
        BoomObject boom;
        if (collision.transform.TryGetComponent(out boom))
        {
            boom.transform.position = To.transform.position;
            StartCoroutine(CooldownCoroutine());
            To.StartCoroutine(To.CooldownCoroutine());
        }
    }

    IEnumerator CooldownCoroutine()
    {
        inCooldown = true;
        yield return new WaitForSeconds(cooldown);

        inCooldown = false;
    }
}
