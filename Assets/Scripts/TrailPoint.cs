using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailPoint : MonoBehaviour
{
    public CharaSO character;
    public PlayerOV target;

    void FixedUpdate()
    {
        Vector3 s = target.animV.normalized;
        Vector3 v = new Vector3(( (s.x * character.followDistance) *-1f), target.transform.position.y, (s.y*character.followDistance) * -1f);
        this.transform.position = Vector3.Lerp(this.transform.position, target.transform.position+v, character.selfIA.speed * Time.deltaTime);
    }
}
