using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Util;

[ExecuteAlways]
public class LoadMatch : MonoBehaviour
{
    [SerializeField] private GameObject[] fieldPrefab;
    [SerializeField] private Transform spawnPoint;
    [Header("Robot Selection")]
    [SerializeField] private InspectorDropdown robotSelected;

    [SerializeField] private Cameras view;
    private int selectedRobotIndex;
    private string selectedName;
    private List<GameObject> availableRobots = new List<GameObject>();


    private GameObject _fieldHolder;
    private GameObject _activeRobot;
    private GameObject _1StCam;

    private FMS fms;

    private void OnEnable()
    {
        CheckRobots();
        robotSelected.canBeSelected = availableRobots.Select(x => x.name).ToList();
    }

    private void LateUpdate()
    {
        CheckRobots();
        robotSelected.canBeSelected = availableRobots.Select(x => x.name).ToList();
        robotSelected.selectedIndex = selectedRobotIndex;
        robotSelected.selectedName = selectedName;
    }

    private void Start()
    {
        selectedName = robotSelected.selectedName;
        selectedRobotIndex = robotSelected.selectedIndex;
        CheckRobots();
        ResetField();
    }

    private void Update()
    {
        selectedName = robotSelected.selectedName;
        selectedRobotIndex = robotSelected.selectedIndex;

        // Editor vs runtime-safe check to avoid referencing editor-only APIs in player builds
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && RobotLoaded())
#else
        if (!Application.isPlaying && RobotLoaded())
#endif
        {
            DeleteRobot();
        }
        if (Application.isPlaying) return;

        if (!CheckField())
        {
            DestroyField();
            LoadField();
        }

        CheckRobots();
    }

    private void LoadField()
    {
        _fieldHolder = new GameObject
        {
            name = "FieldHolder",
            transform = { position = Vector3.zero, rotation = Quaternion.identity, parent = transform },

        };
        Instantiate(fieldPrefab[0], Vector3.zero, Quaternion.identity, _fieldHolder.transform);
    }

    private bool CheckField()
    {
        if (transform.childCount == 0)
        {
            return false;
        }
        else
        {
            return _fieldHolder.transform.Find(fieldPrefab[0].name + "(Clone)");
        }
    }

    private void DestroyField()
    {
        if (transform.Find("FieldHolder"))
        {
            _fieldHolder = transform.Find("FieldHolder").GameObject();
            DestroyImmediate(_fieldHolder);
        }
    }

    public void ResetField()
    {
        DestroyField();
        LoadField();
        SpawnRobot();
        addCamera();
        Utils.resetParentCache();
        if (fms)
        {
            fms.Restart();
        }
    }

    public void setFMS(FMS fms)
    {
        this.fms = fms;
    }

    public GameObject getFieldHolder()
    {
        return _fieldHolder;
    }

    private void SpawnRobot()
    {
        if (availableRobots.Count > 0 && selectedRobotIndex >= 0 && selectedRobotIndex < availableRobots.Count)
        {
            GameObject robotToSpawn = availableRobots[selectedRobotIndex];
            _activeRobot = Instantiate(robotToSpawn, spawnPoint.position, spawnPoint.rotation, _fieldHolder.transform);
            var frame = _activeRobot.GetComponent<BuildFrame>();
            var controller = frame.GetSwerveController();
            if (controller)
            {
                switch (view)
                {
                    case (Cameras.FirstPerson):
                        controller.reversed = false;
                        controller.fieldCentric = true;
                        break;
                    case (Cameras.FirstPersonReversed):
                        controller.reversed = true;
                        controller.fieldCentric = true;
                        break;
                    case (Cameras.ThirdPerson):
                        controller.reversed = false;
                        controller.fieldCentric = true;
                        break;
                    case (Cameras.ReversedThirdPerson):
                        controller.reversed = true;
                        controller.fieldCentric = true;
                        break;
                }
            }
        }
    }

    private bool RobotLoaded()
    {
        return _activeRobot != null;
    }

    public GameObject GetRobotLoaded()
    {
        return _activeRobot;
    }
    private void DeleteRobot()
    {
        DestroyImmediate(_activeRobot);
    }

    private void addCamera()
    {
        string objectToLoad = "Cameras/" + view.ToString();
        _1StCam = Resources.Load(objectToLoad) as GameObject;

        var cam = Instantiate(_1StCam, Vector3.zero, spawnPoint.rotation, _activeRobot.transform); ;
        cam.transform.localPosition = Vector3.zero;
    }


    public void CheckRobots()
    {
        GameObject[] loadedRobots = Resources.LoadAll<GameObject>("Robots");

        availableRobots.Clear();
        foreach (var robot in loadedRobots)
        {
            availableRobots.Add(robot);
        }

        if (selectedRobotIndex >= availableRobots.Count)
        {
            selectedRobotIndex = availableRobots.Count > 0 ? availableRobots.Count - 1 : 0;
        }
    }
}

