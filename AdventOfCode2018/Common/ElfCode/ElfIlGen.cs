using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace AdventOfCode2018.Common.ElfCode
{
    public abstract class ElfIlGen<T> where T : Delegate
    {
        protected ElfIlGen(int pc)
        {
            ProgramCounterReg = pc;
            var invokeMethod = typeof(T).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
            var invokeParams = invokeMethod.GetParameters();
            var invokeParamTypes = new Type[invokeParams.Length];
            for (var i = 0; i < invokeParams.Length; i++) invokeParamTypes[i] = invokeParams[i].ParameterType;
            MethodBuilder = new DynamicMethod("ElfImpl", invokeMethod.ReturnType, invokeParamTypes);
            Gen = MethodBuilder.GetILGenerator();
            Regs = Gen.DeclareLocal(typeof(long[]));
        }

        private DynamicMethod MethodBuilder { get; }
        protected ILGenerator Gen { get; }
        protected LocalBuilder Regs { get; }
        private List<ElfInstruction> ElfInstructions { get; } = new List<ElfInstruction>();
        private int ProgramCounterReg { get; }

        public T Build()
        {
            EmitBegin();
            var labels = new Label[ElfInstructions.Count];
            for (var i = 0; i < ElfInstructions.Count; i++) labels[i] = Gen.DefineLabel();

            var mainSwitch = Gen.DefineLabel();
            var halt = Gen.DefineLabel();
            Gen.MarkLabel(mainSwitch);
            EmitNumGet(ElfOpCode.NumGet.Reg, ProgramCounterReg);
            Gen.Emit(OpCodes.Switch, labels);
            Gen.Emit(OpCodes.Br, halt);

            for (var i = 0; i < labels.Length; i++)
            {
                Gen.MarkLabel(labels[i]);
                var ins = ElfInstructions[i];
                EmitBeforeOp(ins.A, ins.B, ins.C);
                Gen.EmitLoadLocal(Regs);
                Gen.EmitInt(ins.C);
                EmitNumGet(ins.Op.A, ins.A);
                EmitNumGet(ins.Op.B, ins.B);
                EmitOp(ins.Op.Op);
                Gen.Emit(OpCodes.Stelem_I8);
                EmitAfterOp(ins.A, ins.B, ins.C);
                Gen.EmitLoadLocal(Regs);
                Gen.EmitInt(ProgramCounterReg);
                Gen.EmitLoadLocal(Regs);
                Gen.EmitInt(ProgramCounterReg);
                Gen.Emit(OpCodes.Ldelem_I8);
                Gen.EmitInt(1);
                Gen.Emit(OpCodes.Add_Ovf);
                Gen.Emit(OpCodes.Stelem_I8);
                if (ins.C == ProgramCounterReg) Gen.Emit(OpCodes.Br, mainSwitch);
            }

            Gen.MarkLabel(halt);
            EmitEnd();
            return (T) MethodBuilder.CreateDelegate(typeof(T));
        }

        public void AddOpcode(ElfOpCode op, int a, int b, int c)
        {
            ElfInstructions.Add(new ElfInstruction(op, a, b, c));
        }

        public void AddInstruction(ElfInstruction ins)
        {
            ElfInstructions.Add(ins);
        }

        private void EmitNumGet(ElfOpCode.NumGet get, int val)
        {
            switch (get)
            {
                case ElfOpCode.NumGet.Reg:
                    Gen.EmitLoadLocal(Regs);
                    Gen.EmitInt(val);
                    Gen.Emit(OpCodes.Ldelem_I8);
                    return;
                case ElfOpCode.NumGet.Val:
                    Gen.EmitInt(val);
                    return;
                case ElfOpCode.NumGet.Null:
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(get), get, null);
            }
        }

        private void EmitOp(ElfOpCode.Ops op)
        {
            switch (op)
            {
                case ElfOpCode.Ops.Add:
                    Gen.Emit(OpCodes.Add_Ovf);
                    return;
                case ElfOpCode.Ops.Mul:
                    Gen.Emit(OpCodes.Mul_Ovf);
                    return;
                case ElfOpCode.Ops.And:
                    Gen.Emit(OpCodes.And);
                    return;
                case ElfOpCode.Ops.Or:
                    Gen.Emit(OpCodes.Or);
                    return;
                case ElfOpCode.Ops.Set:
                    return;
                case ElfOpCode.Ops.Greater:
                    Gen.Emit(OpCodes.Cgt);
                    return;
                case ElfOpCode.Ops.Equal:
                    Gen.Emit(OpCodes.Ceq);
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }

        protected virtual void EmitAfterOp(int a, int b, int c)
        {
        }

        protected virtual void EmitBeforeOp(int a, int b, int c)
        {
        }

        protected abstract void EmitBegin();
        protected abstract void EmitEnd();
    }
}