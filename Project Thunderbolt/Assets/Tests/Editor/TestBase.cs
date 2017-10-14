using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBase {
    protected string GAMEOBJECT = "TestGameObject";

    protected GameObject go() {
        return new GameObject(this.GAMEOBJECT);
    }

    protected GameObject getPlayer() {
        return (GameObject) UnityEngine.Object.Instantiate(Resources.Load("prefabs/CharacterRobotBoy"));
    }
}
