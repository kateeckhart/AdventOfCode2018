using System.Reflection.Emit;

namespace AdventOfCode2018.Common.ElfCode
{
    public static class IlGen
    {
        public static void EmitInt(this ILGenerator gen, int n)
        {
            switch (n)
            {
                case -1:
                    gen.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    gen.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    gen.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    gen.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    gen.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    gen.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    gen.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    gen.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (n >= sbyte.MinValue && n <= sbyte.MaxValue)
                        gen.Emit(OpCodes.Ldc_I4_S, (sbyte) n);
                    else
                        gen.Emit(OpCodes.Ldc_I4, n);

                    break;
            }
        }

        public static void EmitLoadLocal(this ILGenerator gen, LocalBuilder local)
        {
            switch (local.LocalIndex)
            {
                case 0:
                    gen.Emit(OpCodes.Ldloc_0);
                    return;
                case 1:
                    gen.Emit(OpCodes.Ldloc_1);
                    return;
                case 2:
                    gen.Emit(OpCodes.Ldloc_2);
                    return;
                case 3:
                    gen.Emit(OpCodes.Ldloc_3);
                    return;
                default:
                    if (local.LocalIndex <= byte.MaxValue)
                        gen.Emit(OpCodes.Ldloc_S, (byte) local.LocalIndex);
                    else
                        gen.Emit(OpCodes.Ldloc, (ushort) local.LocalIndex);
                    return;
            }
        }

        public static void EmitStoreLocal(this ILGenerator gen, LocalBuilder local)
        {
            switch (local.LocalIndex)
            {
                case 0:
                    gen.Emit(OpCodes.Stloc_0);
                    return;
                case 1:
                    gen.Emit(OpCodes.Stloc_1);
                    return;
                case 2:
                    gen.Emit(OpCodes.Stloc_2);
                    return;
                case 3:
                    gen.Emit(OpCodes.Stloc_3);
                    return;
                default:
                    if (local.LocalIndex <= byte.MaxValue)
                        gen.Emit(OpCodes.Stloc_S, (byte) local.LocalIndex);
                    else
                        gen.Emit(OpCodes.Stloc, (ushort) local.LocalIndex);
                    return;
            }
        }
    }
}