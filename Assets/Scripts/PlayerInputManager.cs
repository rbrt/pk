using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;

public class PlayerInputManager : MonoBehaviour {

	static PlayerInputManager instance;
	protected static int minLifetime = 6;	// Frames to wait before processing anything

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

		AttackTree.AttackInputType inputType = ProcessInput();

		if (inputType != AttackTree.AttackInputType.None){
			// Do something
		}
	}

	void UpdateFrames(){
		inputList.ForEach(element => element.frames++);
	}

	AttackTree.AttackInputType ProcessInput(){
		if (inputList.Count == 0){
			return AttackTree.AttackInputType.None;
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
							return AttackTree.AttackInputType.DownToForwardA1;
						}
						// Down Right Attack2
						else if(inputList[0].ValidInput(InputTypes.Attack2)){
							return AttackTree.AttackInputType.DownToForwardA2;
						}
					}

				}
				// Down Left
				else if (inputList[0].ValidInput(InputTypes.Left)){

					inputList.RemoveAt(0);
					if (inputList.Count > 0){
						// Down Left Attack1
						if(inputList[0].ValidInput(InputTypes.Attack1)){
							return AttackTree.AttackInputType.DownToBackA1;
						}
						// Down Left Attack2
						else if(inputList[0].ValidInput(InputTypes.Attack2)){
							return AttackTree.AttackInputType.DownToBackA2;
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
							return AttackTree.AttackInputType.ForwardForwardA1;
						}
						// Right Right Attack2
						else if(inputList[0].ValidInput(InputTypes.Attack2)){
							return AttackTree.AttackInputType.ForwardForwardA2;
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
							return AttackTree.AttackInputType.BackBackA1;
						}
						// Left Left Attack2
						else if(inputList[0].ValidInput(InputTypes.Attack2)){
							return AttackTree.AttackInputType.BackBackA2;
						}
					}
				}
			}
		}
		// Attack1
		else if (inputList[0].ValidInput(InputTypes.Attack1)){
			return AttackTree.AttackInputType.Attack1;
		}
		// Attack2
		else if (inputList[0].ValidInput(InputTypes.Attack2)){
			return AttackTree.AttackInputType.Attack2;
		}

		return AttackTree.AttackInputType.None;
	}

	public struct InputPair {
		public InputTypes inputType;
		public int frames;

		public InputPair(InputTypes newInput){
			inputType = newInput;
			frames = 0;
		}
	}
}

namespace ExtensionMethods{
    public static class InputPairExtensions{
        public static bool ValidInput(this PlayerInputManager.InputPair pair, PlayerInputManager.InputTypes inputType){
			return (pair.frames >= PlayerInputManager.MinLifetime && inputType == pair.inputType);
		}
    }
}
