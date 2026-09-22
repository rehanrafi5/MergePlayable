using UnityEngine;

public class MultiplierSetter : MonoBehaviour
{
   [SerializeField] private int MultiplierValue;

   public int GetValue()
   {
      return MultiplierValue;
   }
}
