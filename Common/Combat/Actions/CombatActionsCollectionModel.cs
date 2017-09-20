using Common.Serialization;
using System.Collections.Generic;

namespace Common.Combat.Actions
{
    /// <summary>
    /// A collection of combat actions performed by/to an individual CombatantModel.
    /// Main data stucture for sending combat info from server to all clients.
    /// </summary>
    public class CombatActionsCollectionModel : Dictionary<int, List<CombatActionModel>>, ISerializableModel
    {
        public int TurnNumber;

        public bool HasEntries()
        {
            foreach (var kvp in this)
            {
                if (kvp.Value.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void FromObjectArray(object[] properties)
        {
            Clear();
            int index = 0;
            TurnNumber = (int)properties[index++];

            object[] combatantIDAndActionListGroup = properties[index++] as object[];
            for (int i = 0; i < combatantIDAndActionListGroup.Length; i++)
            {
                object[] combatantIDAndActionsList = combatantIDAndActionListGroup[i] as object[];
                int combatantID = (int)combatantIDAndActionsList[0];
                Add(combatantID, new List<CombatActionModel>());
                for (int j = 1; j < combatantIDAndActionsList.Length; j++)
                {
                    CombatActionModel combatActionModel = CombatActionModel.FromObjectArray(combatantIDAndActionsList[j] as object[]);
                    this[combatantID].Add(combatActionModel);
                }
            }
        }

        public object[] ToObjectArray()
        {
            int index = 0;

            object[] output = new object[2];
            output[index++] = TurnNumber;

            int numEntries = 0;
            foreach (var kvp in this)
            {
                if (kvp.Value.Count > 0)
                {
                    numEntries++;
                }
            }
            object[] combatantIDAndActionListGroup = new object[numEntries];
            int j = 0;
            foreach (var kvp in this)
            {
                // Don't add to object[] if the combatant doesn't have any actions
                List<CombatActionModel> combatActionModels = kvp.Value;
                if (combatActionModels.Count == 0)
                {
                    continue;
                }

                int combatantID = kvp.Key;
                object[] combatantIDAndActionsList = new object[combatActionModels.Count + 1];
                combatantIDAndActionsList[0] = combatantID;

                for (int i = 0; i < combatActionModels.Count; i++)
                {
                    combatantIDAndActionsList[i + 1] = CombatActionModel.ToObjectArray(combatActionModels[i]);
                }

                combatantIDAndActionListGroup[j++] = combatantIDAndActionsList;
            }

            output[index++] = combatantIDAndActionListGroup;
            return output;
        }

        public override string ToString()
        {
            string output = "<CombatActionsCollectionModel> TurnNumber: " + TurnNumber+ "\n";
            foreach (var kvp in this)
            {
                output += "   Combatant ID: " + kvp.Key + " : \n";
                for (int i= 0; i < kvp.Value.Count; i++)
                {
                    output += "      " + kvp.Value[i].ToString() + "\n";
                }
            }
            return output;
        }
    }
}
