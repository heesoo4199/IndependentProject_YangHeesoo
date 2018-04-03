using UnityEngine;
using System.Collections;

public interface ProjectileLauncher
{
    void CreateNewProjectile();
    bool isShot();
    void changeShot(bool a);
    string toString();
}
