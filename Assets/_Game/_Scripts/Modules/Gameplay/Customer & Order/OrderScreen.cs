using System.Collections.Generic;
using DQHieu.Framework;
using UnityEngine;

public class OrderScreen : MonoBehaviour
{
    public List<OrderTicketUI> orderTickets = new();
    public OrderTicketUI activeTicket;

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void Update()
    {
        HandleOrderScreenInput();
    }

    private void HandleOrderScreenInput()
    {
        for(int i = 0; i < orderTickets.Count; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            if(Input.GetKeyDown(key))
            {
                if(activeTicket == null)
                {
                    activeTicket = orderTickets[i];
                    activeTicket.Show();
                }
                else
                {
                    if(activeTicket == orderTickets[i])
                    {
                        activeTicket.Hide();
                        activeTicket = null;
                    }
                    else
                    {
                        activeTicket.Hide();
                        activeTicket = orderTickets[i];
                        activeTicket.Show();
                    }
                }
            }
        }
    }

    public void ShowScreen()
    {
        
    }

    public void HideScreen()
    {
        
    }



}
