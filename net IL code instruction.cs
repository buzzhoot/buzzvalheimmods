//?Ldc_I4_? int 0-8
//?Ldc_I4_S,? int >8

Opcodes.call methodInfo = AccessTools.Method(typeof(Random), nameof(Random.Range), new System.Type[] { typeof(int), typeof(int)});

var methodInfo = AccessTools.Method(typeof(Random), nameof(Random.Range), new System.Type[] { typeof(int), typeof(int)});
var codes = instructions.ToList();

[HarmonyTranspiler]
[HarmonyPatch(typeof(Player), nameof(Player.SetControls))]
//!public static IEnumerable<CodeInstruction> SetControlsTranspiler(IEnumerable<CodeInstruction> instructions){
foreach(var code in codes)
{
    if (code.opcode == OpCodes.Ldc_I4_S) //当迭代到OpCodes.Ldc_I4_S时判断operand
    {
        if ((sbyte)code.operand == 10) //注意，这里operand的类型为sbyte，不是int
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_7); //压入7
            yield return new CodeInstruction(OpCodes.Ldc_I4_S, 14); //压入14
            yield return new CodeInstruction(OpCodes.Call, methodInfo); //压入随机函数
        }
        if ((sbyte)code.operand == 20)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_S, 17);
            yield return new CodeInstruction(OpCodes.Ldc_I4_S, 24);
            yield return new CodeInstruction(OpCodes.Call, methodInfo);
        }
    }
    else
    {
        yield return code;
    }
}
//--}
//作者：宵夜97
//https://www.bilibili.com/read/cv9104048
//出处： bilibili

 [HarmonyPatch]
    public static class Patches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Player), nameof(Player.SetControls))]
        public static IEnumerable<CodeInstruction> SetControlsTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool foundCancelCrouch = false;
            bool foundAutoRunCondition = false;
            bool foundMoveDirCondition = false;
            bool foundCancelAutorunCondition = false;
            bool foundMoveDirAssignment = false;
            CodeInstruction branchEndAutoRun = null;

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            //DumpIL(codes);
            for (int i = 0; i < codes.Count(); i++)
            {
                CodeInstruction code = codes[i];

                if (!foundCancelCrouch
                    && codes[i + 0].opcode == OpCodes.Ldarg_0
                    && codes[i + 1].opcode == OpCodes.Ldc_I4_0
                    && codes[i + 2].Calls(AccessTools.Method(typeof(Character), nameof(Character.SetCrouch))))
                {
                    //Debug.Log("foundit0 " + i);
                    // Nop out the call to "this.SetCrouch(false)"
                    codes[i + 0].opcode = OpCodes.Nop; codes[i + 0].operand = null;
                    codes[i + 1].opcode = OpCodes.Nop; codes[i + 1].operand = null;
                    codes[i + 2].opcode = OpCodes.Nop; codes[i + 2].operand = null;

                    foundCancelCrouch = true;
                }

                // We're looking for a ldfld of m_autoRun followed by a brfalse: "if(this.m_autoRun)"
                if (!foundAutoRunCondition
                    && code.opcode == OpCodes.Ldfld 
                    && code.LoadsField(AccessTools.DeclaredField(typeof(Player), nameof(Player.m_autoRun)))
                    && codes[i + 1].opcode == OpCodes.Brfalse)
                {
                    //Debug.Log("foundit1 " + i);
                    foundAutoRunCondition = true;
                }

                // Nop out the "jump || crouch || movedir != Vector3.zero" conditions
                if (foundAutoRunCondition
                    && codes[i - 5].opcode == OpCodes.Ldarg_S
                    && codes[i - 4].opcode == OpCodes.Or
                    && codes[i - 3].opcode == OpCodes.Ldarg_S
                    && codes[i - 2].opcode == OpCodes.Or
                    && codes[i - 1].Branches(out Label? asdfasdfasdsa)
                    && codes[i + 0].opcode == OpCodes.Ldarg_1
                    && codes[i + 1].opcode == OpCodes.Call)
                {
                    //Debug.Log("foundit3 " + i);
                    codes[i - 5].opcode = OpCodes.Nop; codes[i - 5].operand = null;
                    codes[i - 4].opcode = OpCodes.Nop; codes[i - 4].operand = null;
                    codes[i - 3].opcode = OpCodes.Nop; codes[i - 3].operand = null;
                    codes[i - 2].opcode = OpCodes.Nop; codes[i - 2].operand = null;
                    // Leave codes[i - 1] alone since it's the branching instruction for the very first condition
                    codes[i + 0].opcode = OpCodes.Nop; codes[i + 0].operand = null;
                    codes[i + 1].opcode = OpCodes.Nop; codes[i + 1].operand = null;
                    codes[i + 2].opcode = OpCodes.Nop; codes[i + 2].operand = null;
                    codes[i + 3].opcode = OpCodes.Nop; codes[i + 3].operand = null;

                    // Add in our own autorun canceling conditions: if either forwards or backwards are pressed.
                    branchEndAutoRun = codes[i - 1];
                    codes.InsertRange(i, new List<CodeInstruction>() {
                        new CodeInstruction(OpCodes.Ldstr, "Forward"),
                        CodeInstruction.Call(typeof(ZInput), nameof(ZInput.GetButton)),
                        branchEndAutoRun.Clone(),
                        new CodeInstruction(OpCodes.Ldstr, "Backward"),
                        CodeInstruction.Call(typeof(ZInput), nameof(ZInput.GetButton)),
                        branchEndAutoRun.Clone()
                    });

                    foundCancelAutorunCondition = true;
                }

                // Convert "else if (autoRun || blockHold)" into "else"
                if (foundCancelAutorunCondition
                    && codes[i + 0].opcode == OpCodes.Ldarg_S //&& codes[i + 0].operand as string == "10"
                    && codes[i + 1].opcode == OpCodes.Ldarg_S //&& codes[i + 1].operand as string == "6"
                    && codes[i + 2].opcode == OpCodes.Or
                    && codes[i + 3].Branches(out Label? asdfasdfsad))
                {
                    //Debug.Log("foundit4 " + i);
                    codes[i + 0].opcode = OpCodes.Nop; codes[i + 0].operand = null;
                    codes[i + 1].opcode = OpCodes.Nop; codes[i + 1].operand = null;
                    codes[i + 2].opcode = OpCodes.Nop; codes[i + 2].operand = null;
                    codes[i + 3].opcode = OpCodes.Nop; codes[i + 3].operand = null;

                    foundMoveDirCondition = true;
                }

                // Lastly, add "movedir.x * Vector3.Cross(Vector3.up, this.m_lookDir)" to the player's movedir so that they can  while autorunning
                if (foundMoveDirCondition
                    && codes[i - 3].opcode == OpCodes.Ldarg_0
                    && codes[i - 2].opcode == OpCodes.Ldarg_0
                    && codes[i - 1].LoadsField(AccessTools.Field(typeof(Character), nameof(Character.m_lookDir)))
                    && codes[i - 0].StoresField(AccessTools.Field(typeof(Character), nameof(Character.m_moveDir))))
                {
                    //Debug.Log("foundit5 " + i);
                    codes.InsertRange(i, new List<CodeInstruction>() {
                        new CodeInstruction(OpCodes.Ldarg_1),
                        CodeInstruction.LoadField(typeof(Vector3), nameof(Vector3.x)),
                        new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Vector3), nameof(Vector3.up))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        CodeInstruction.LoadField(typeof(Player), nameof(Player.m_lookDir)),
                        CodeInstruction.Call(typeof(Vector3), nameof(Vector3.Cross)),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Vector3), "op_Multiply", new Type[] { typeof(float), typeof(Vector3) })),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Vector3), "op_Addition", new Type[] { typeof(Vector3), typeof(Vector3) }))
                    });

                    foundMoveDirAssignment = true;
                    break; // we done
                }
            }
            
            //DumpIL(codes);

            //Debug.Log(string.Format("{0} {1} {2} {3} {4}", foundAutoRunCondition, branchEndAutoRun != null, foundCancelAutorunCondition, foundMoveDirCondition, foundMoveDirAssignment));
            if (!foundAutoRunCondition || branchEndAutoRun == null || !foundCancelAutorunCondition || !foundMoveDirCondition || !foundMoveDirAssignment)
                throw new Exception("BetterAutoRun injection point NOT found!! Game has most likely updated and broken this mod!");

            if (!foundCancelCrouch)
            {
                Main.log.LogWarning("One of the BetterAutoRun injection points were not found, game has most likely updated and broken this mod.");
                Main.log.LogWarning("Autorun while crouching will not work but everything else should be fine.");
            }

            return codes.AsEnumerable();
        }
    }