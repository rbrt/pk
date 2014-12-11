using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;

public class PlayerInputManager : MonoBehaviour {

	static PlayerInputManager instance;
	protected static int minLifetime = 5;	// Frames to wait before processing anything
	protected static int ttl = 25;			// Time for an item to remain in the queue

	public enum InputTypes {Up, Right, Left, Down, Attack1, Attack2, Block};

	List<InputPair> inputList;

	public static PlayerInputManager InputManager {
		get { return instance; }
	}

	public static float MinLifetime {
		get { return minLifetime; }
	}

	public void SendInput(InputTypes inputType){
		inputList.Add(new InputPair(inputType));
	}

	void Awake () {
		inputList = new List<InputPair>();
		instance = this;
	}

	void Update () {
		UpdateFrames();

		AttackHandler.AttackInputType inputType = ProcessInput();

		if (inputType != AttackHandler.AttackInputType.None){
			PlayerController.CurrentPlayerAttack = inputType;
		}
	}

	void UpdateFrames(){
		for (int i = inputList.Count-1; i >= 0; i--){
			var element = inputList[i];
			element.frames--;

			if (element.frames < 0){
				inputList.RemoveAt(i);
			}
			else{
				inputList[i] = element;
			}
		}
	}

	AttackHandler.AttackInputType ProcessInput(){
		if (inputList.Count == 0){
			return AttackHandler.AttackInputType.None;
		}

		// Wait for minLifetime frames before grabbing an input batch
		if (inputList[0].frames > minLifetime){
			return AttackHandler.AttackInputType.None;
		}

		// Down
		if (inputList[0].ValidInput(InputTypes.Down)){
			inputList.RemoveAt(0);
			if (inputList.Count > 0){
				// Down Right
				if(inputList[0].ValidInput(InputTypes.Right)){

					inputList.RemoveAt(0);
					if (inputList.Count > 0){
						// Down Right Attack1
						if(inputList[0].ValidInput(InputTypes.Attack1)){
							return AttackHandler.AttackInputType.DownToForwardA1;
						}
						// Down Right Attack2
						else if(inputList[0].ValidInput(InputTypes.Attack2)){
							return AttackHandler.AttackInputType.DownToForwardA2;
						}
					}

				}
				// Down Left
				else if (inputList[0].ValidInput(InputTypes.Left)){

					inputList.RemoveAt(0);
					if (inputList.Count > 0){
						// Down Left Attack1
						if(inputList[0].ValidInput(InputTypes.Attack1)){
							return AttackHandler.AttackInputType.DownToBackA1;
						}
						// Down Left Attack2
						else if(inputList[0].ValidInput(InputTypes.Attack2)){
							return AttackHandler.AttackInputType.DownToBackA2;
						}
					}

				}

			}
		}
		// Right
		else if (inputList[0].ValidInput(InputTypes.Right)){
			inputList.RemoveAt(0);
			if (inputList.Count > 0){
				// Right Right
				if(inputList[0].ValidInput(InputTypes.Right)){
					inputList.RemoveAt(0);
					if (inputList.Count > 0){

						// Right Right Attack1
						if(inputList[0].ValidInput(InputTypes.Attack1)){
							return AttackHandler.AttackInputType.ForwardForwardA1;
						}
						// Right Right Attack2
						else if(inputList[0].ValidInput(InputTypes.Attack2)){
							return AttackHandler.AttackInputType.ForwardForwardA2;
						}
					}
				}
			}
		}
		// Left
		else if (inputList[0].ValidInput(InputTypes.Left)){
			inputList.RemoveAt(0);
			if (inputList.Count > 0){
				// Left Left
				if(inputList[0].ValidInput(InputTypes.Left)){
					inputList.RemoveAt(0);
					if (inputList.Count > 0){
						// Left Left Attack1
						if(inputList[0].ValidInput(InputTypes.Attack1)){
							Debug.Log("Special!");
							return AttackHandler.AttackInputType.BackBackA1;
						}
						// Left Left Attack2
						else if(inputList[0].ValidInput(InputTypes.Attack2)){
							return AttackHandler.AttackInputType.BackBackA2;
						}
					}
				}
			}
		}
		// Attack1
		else if (inputList[0].ValidInput(InputTypes.Attack1)){
			inputList.RemoveAt(0);
			return AttackHandler.AttackInputType.Attack1;
		}
		// Attack2
		else if (inputList[0].ValidInput(InputTypes.Attack2)){
			inputList.RemoveAt(0);
			return AttackHandler.AttackInputType.Attack2;
		}

		return AttackHandler.AttackInputType.None;
	}

	public struct InputPair {
		public InputTypes inputType;
		public int frames;

		public InputPair(InputTypes newInput){
			inputType = newInput;
			frames = ttl;
		}
	}
}

namespace ExtensionMethods{
    public static class InputPairExtensions{
        public static bool ValidInput(this PlayerInputManager.InputPair pair, PlayerInputManager.InputTypes inputType){
			return (inputType == pair.inputType);
		}
    }
}
