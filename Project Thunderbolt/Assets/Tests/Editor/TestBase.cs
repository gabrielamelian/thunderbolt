using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestBase {
    protected string GAMEOBJECT = "TestGameObject";

    protected GameObject go() {
        return new GameObject(this.GAMEOBJECT);
    }

    protected GameObject getPlayer() {
        return (GameObject) UnityEngine.Object.Instantiate(AssetDatabase.LoadMainAssetAtPath("Assets/Standard Assets/2D/Prefabs/CharacterRobotBoy.prefab"));
    }
}
