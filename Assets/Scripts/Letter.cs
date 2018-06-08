﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Letter : BaseObject
{

    #region Public

    public List<GameObject> UpBridge;
    public List<GameObject> RightBridge;

    //[HideInInspector]
    public List<Letter> ConnectedLetters;

    public bool IsSelected = false;
    #endregion

    #region Events

    public UnityEvent OnSelect;
    public UnityEvent OnUnSelect;
    public UnityEvent OnSpawn;

    #endregion

    #region Private

    private Vector3 _lastDragPos;
    private Renderer[] _renderers;
    public bool Active=true;
    public GameObject Frame;
    public char Char;

    #endregion

    #region Start

    void Start()
    {
        _renderers= GetComponentsInChildren<Renderer>();
        OnSpawn.Invoke();
    }

    #endregion

    #region SetupBridges

    public void SetupBridges()
    {
        bool ub = false, rb = false;

        foreach (Letter letter in ConnectedLetters)
        {
            if ((letter.transform.position - transform.position - Vector3.up).magnitude<0.2f)
                ub = true;

            if ((letter.transform.position - transform.position - Vector3.right).magnitude<0.2f)
                rb = true;
        }

        foreach (GameObject rbGameObject in RightBridge)
            rbGameObject.SetActive(rb);

        foreach (GameObject ubGameObject in UpBridge)
            ubGameObject.SetActive(ub);

    }
    #endregion

    #region GetConnectedLetters

    public void GetConnectedLetters(List<Letter> letters)
    {
        if (letters.Contains(this))
            return;

        letters.Add(this);

        foreach (Letter letter in ConnectedLetters)
            letter.GetConnectedLetters(letters);
    }
    #endregion

    #region Select/Unselected

    public void Select()
    {
        IsSelected = true;
        OnSelect.Invoke();
    }

    public void Unselect()
    {
        IsSelected = false;
        OnUnSelect.Invoke();
    }

    #endregion

    #region Move

    public void Move(Vector3 delta)
    {
        transform.position += delta;
    }


    #endregion

    #region Snap

    public void Snap()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        transform.position = position;
    }
    #endregion

    public void AddToBounds(ref Bounds bound)
    {
        foreach (Renderer renderer1 in _renderers)
            bound.Encapsulate(renderer1.bounds);
    }

    public bool IsNextTo(Letter letter, Direction direction)
    {
        if (direction == Direction.Horizontal)
            return 
                Math.Abs(transform.position.x - (letter.transform.position.x + 1)) < 0.1f &&
                Math.Abs(transform.position.y - letter.transform.position.y) < 0.1f;
        else
            return
                Math.Abs(transform.position.x - letter.transform.position.x) < 0.1f &&
                Math.Abs(transform.position.y - (letter.transform.position.y + 1)) < 0.1f;
    }

    public void ConnectTo(Letter letter)
    {
        if (!ConnectedLetters.Contains(letter))
        {
            ConnectedLetters.Add(letter);
            SetupBridges();
            letter.ConnectTo(this);
        }
    }

    public void SetCharacter(char c)
    {
        GetComponentInChildren<TextMesh>().text = c.ToString();
    }

    public void DisConnect(Letter l2)
    {
        if (ConnectedLetters.Contains(l2))
        {
            ConnectedLetters.Remove(l2);
            l2.DisConnect(this);
        }
    }

    public char GetCharacter()
    {
        return GetComponentInChildren<TextMesh>().text[0];
    }
}
