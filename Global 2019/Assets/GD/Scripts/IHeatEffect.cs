using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeatEffect
{
    void CheckParentIsHeatable();

    void StartHeatEffect();

    void StopHeatEffect();
}