using UnityEngine;
using System.Collections.Generic;

public class BotMergeManager : MonoBehaviour
{
    [System.Serializable]
    public struct BotData
    {
        public int level;
        public GameObject botPrefab;
    }

    [Header("Bot Configuration")]
    public List<BotData> botForms;
    public float dragHeightOffset = 1.0f;

    [Header("Slots Configuration")]
    public List<Transform> botSlots; 
    private Dictionary<Transform, GameObject> slotOccupancy = new Dictionary<Transform, GameObject>();

    private GameObject selectedBot;
    private Transform originalSlot;
    private Plane dragPlane;
    private GameplayHandler gameManager; // Game state track karne ke liye

    void Start()
    {
        gameManager = FindObjectOfType<GameplayHandler>();
        InitializeSlots();
    }

    void InitializeSlots()
    {
        foreach (Transform slot in botSlots)
        {
            slotOccupancy[slot] = null;
        }

        BotIdentity[] existingBots = FindObjectsOfType<BotIdentity>();
        foreach (var bot in existingBots)
        {
            Transform closestSlot = GetClosestSlot(bot.transform.position);
            if (closestSlot != null)
            {
                slotOccupancy[closestSlot] = bot.gameObject;
                bot.transform.position = closestSlot.position; 
            }
        }
    }

    void Update()
    {
        // FIX: Agar game 'Fighting' state mein hai, toh merge/drag disable rahay ga
        if (gameManager != null && gameManager.currentState == GameplayHandler.GameState.Fighting)
        {
            return;
        }

        HandleTouchOrClick();
    }

    void HandleTouchOrClick()
    {
        // --- MOUSE DOWN: Select Bot ---
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Bot"))
                {
                    selectedBot = hit.collider.gameObject;
                    originalSlot = GetBotSlot(selectedBot);

                    dragPlane = new Plane(Vector3.up, selectedBot.transform.position);
                }
            }
        }

        // --- MOUSE DRAG: Move with finger ---
        if (Input.GetMouseButton(0) && selectedBot != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (dragPlane.Raycast(ray, out enter))
            {
                Vector3 worldPos = ray.GetPoint(enter);
                worldPos.y += dragHeightOffset; 
                selectedBot.transform.position = worldPos;
            }
        }

        // --- MOUSE UP: Drop & Check Slots ---
        if (Input.GetMouseButtonUp(0) && selectedBot != null)
        {
            Transform targetSlot = GetClosestSlot(selectedBot.transform.position);

            if (targetSlot != null)
            {
                GameObject residentBot = slotOccupancy[targetSlot];

                if (residentBot == null)
                {
                    slotOccupancy[originalSlot] = null;
                    slotOccupancy[targetSlot] = selectedBot;
                    selectedBot.transform.position = targetSlot.position;
                }
                else if (residentBot == selectedBot)
                {
                    selectedBot.transform.position = originalSlot.position;
                }
                else
                {
                    BotIdentity sourceId = selectedBot.GetComponent<BotIdentity>();
                    BotIdentity targetId = residentBot.GetComponent<BotIdentity>();

                    if (sourceId != null && targetId != null && sourceId.level == targetId.level)
                    {
                        int currentLevel = sourceId.level; 
                        int nextLevel = currentLevel; 

                        if (nextLevel >= botForms.Count)
                        {
                            Debug.Log("Max level reached! Cannot merge further.");
                            selectedBot.transform.position = originalSlot.position;
                        }
                        else
                        {
                            Vector3 spawnPos = targetSlot.position;

                            Destroy(selectedBot);
                            Destroy(residentBot);

                            slotOccupancy[originalSlot] = null;
                            slotOccupancy[targetSlot] = null;

                            GameObject newBot = Instantiate(botForms[nextLevel].botPrefab, spawnPos, Quaternion.identity);
                            slotOccupancy[targetSlot] = newBot;
                            Debug.Log("Successfully merged to level " + (currentLevel + 1));
                        }
                    }
                    else
                    {
                        selectedBot.transform.position = originalSlot.position;
                    }
                }
            }
            else
            {
                selectedBot.transform.position = originalSlot.position;
            }

            selectedBot = null;
        }
    }

    Transform GetClosestSlot(Vector3 pos)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform slot in botSlots)
        {
            float dist = Vector3.Distance(pos, slot.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = slot;
            }
        }
        return closest;
    }

    Transform GetBotSlot(GameObject bot)
    {
        foreach (var kvp in slotOccupancy)
        {
            if (kvp.Value == bot) return kvp.Key;
        }
        return null;
    }
}