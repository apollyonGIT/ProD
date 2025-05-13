using Foundations;
using Foundations.Tickers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using World;
using World.BackPack;
using World.Characters;
using World.Widgets;

public class InputController : MonoBehaviourSingleton<InputController>
{
    public Control_1 c1;
    private const int input_priority = 0;
    private const string input_name = "input";
    public event Action tick_action;

    public event Action right_click_event, right_hold_event, right_release_event;
    public event Action left_hold_event, left_release_event;
    public event Action esc_event;
    public event Action r_event;


    protected override void on_init()
    {
        base.on_init();
        {
            c1 = new Control_1();
        }
    }

    public void Del(InputAction.CallbackContext context)
    {
        Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);
        
        if(bmgr.select_slot!= null)
        {
            if (bmgr.select_slot.loot == null)
                return;
            else
            {
                bmgr.RemoveLoot(bmgr.select_slot.loot);
                bmgr.CancelSelectSlot();
            }
        }
    }
    public void SelectCharacter_4(InputAction.CallbackContext context)
    {
        Mission.instance.try_get_mgr(Commons.Config.CharacterMgr_Name, out CharacterMgr cmgr);
        if (cmgr.characters.Count < 4)
        {
            return;
        }

        cmgr.SelectCharacter(cmgr.characters[3]);
    }

    public void SelectCharacter_3(InputAction.CallbackContext context)
    {
        Mission.instance.try_get_mgr(Commons.Config.CharacterMgr_Name, out CharacterMgr cmgr);
        if (cmgr.characters.Count < 3)
        {
            return;
        }

        cmgr.SelectCharacter(cmgr.characters[2]);
    }

    public void SelectCharacter_2(InputAction.CallbackContext context)
    {
        Mission.instance.try_get_mgr(Commons.Config.CharacterMgr_Name, out CharacterMgr cmgr);
        if (cmgr.characters.Count < 2)
        {
            return;
        }

        cmgr.SelectCharacter(cmgr.characters[1]);
    }

    public void SelectCharacter_1(InputAction.CallbackContext context)
    {

        Mission.instance.try_get_mgr(Commons.Config.CharacterMgr_Name, out CharacterMgr cmgr);
        if (cmgr.characters.Count < 1)
        {
            return;
        }

        cmgr.SelectCharacter(cmgr.characters[0]);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:            //长按结束，开始执行逻辑
                if(context.interaction is HoldInteraction)
                {
                      LeftHold(context);
                }
                break;
            case InputActionPhase.Started:              //开始点按或者长按
                break;
            case InputActionPhase.Canceled:
                LeftRelease(context);
                break;
        }
    }

    #region PullUp
    bool holding_pullup;

    private void pull_up()
    {
        Widget_DrivingLever_Context.instance.Drag_Lever(true, true, true);
    }
    public void PullUp(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:            //长按结束，开始执行逻辑
                if (context.interaction is HoldInteraction)
                {
                    holding_pullup = true;
                    tick_action += pull_up;
                }
                else
                {
                    pull_up();
                }
                break;
            case InputActionPhase.Started:              //开始点按或者长按
                break;
            case InputActionPhase.Canceled:
                if (holding_pullup)
                {
                    holding_pullup = false;
                    tick_action -= pull_up;
                }
                break;
        }
    }
    #endregion

    #region PullDown
    bool holding_pulldown;

    private void pull_down()
    {
        Widget_DrivingLever_Context.instance.Drag_Lever(true, false, true);
    }

    public void PullDown(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:            //长按结束，开始执行逻辑
                if (context.interaction is HoldInteraction)
                {
                    holding_pulldown = true;
                    tick_action += pull_down;
                }
                else
                {
                    pull_down();
                }
                break;
            case InputActionPhase.Started:              //开始点按或者长按
                break;
            case InputActionPhase.Canceled:
                if (holding_pulldown)
                {
                    holding_pulldown = false;
                    tick_action -= pull_down;
                }
                break;
        }
    }
    #endregion

    public void Brake(InputAction.CallbackContext context)
    {
        Widget_DrivingLever_Context.instance.SetLever(0, true);
        WorldContext.instance.caravan_status_acc = WorldEnum.EN_caravan_status_acc.braking;
    }

    #region LeftButton
    [HideInInspector]
    public bool holding_left;

    private void LeftHold(InputAction.CallbackContext context)
    {
        holding_left = true;
        tick_action += left_hold_event;
    }

    private void LeftRelease(InputAction.CallbackContext context)
    {
        if (holding_left)
        {
            holding_left = false;
            tick_action -= left_hold_event;
        }
        left_release_event?.Invoke();
    }

    public void RemoveLeftHold(Action action)
    {
        if (holding_left)
        {
            holding_left = false;
            tick_action -= left_hold_event;
        }

        left_hold_event -= action;
    }
    #endregion

    public void RClick(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                r_event?.Invoke();
                Widget_Fix_Context.instance.PlayerOper(!Widget_Fix_Context.instance.player_oper);

                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
                break;
        }
        
    }

    public void PClick(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (Widget_PushCar_Context.instance.AbleToPush_CheckCD)
                    if (Widget_PushCar_Context.instance.AbleToPush())
                        Widget_PushCar_Context.instance.PushCaravan();
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    public Vector2 GetScreenMousePosition()
    {
        return Mouse.current.position.ReadValue();
    }

    public Vector3 GetWorlMousePosition()
    {
        var mousePosition = GetScreenMousePosition();
        var pos = WorldSceneRoot.instance.mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10 - WorldSceneRoot.instance.mainCamera.transform.position.z));

        return pos;
    }

    public void tick()
    {
        tick_action?.Invoke();
    }

    public void OnEnable()
    {
        c1.Enable();
        Ticker.instance.add_tick(input_priority, input_name, tick);
    }
    private void OnDisable()
    {
        c1.Disable();
        Ticker.instance.remove_tick(input_name);
    }
}
