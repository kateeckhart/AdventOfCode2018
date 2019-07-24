using System;
using System.Reflection;
using System.Reflection.Emit;

namespace AdventOfCode2018.Common.ElfCode
{
    public abstract class ElfIlGen<T> where T : Delegate
    {
        private T _method;

        protected ElfIlGen()
        {
            var invokeMethod = typeof(T).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
            var invokeParams = invokeMethod.GetParameters();
            var invokeParamTypes = new Type[invokeParams.Length];
            for (var i = 0; i < invokeParams.Length; i++) invokeParamTypes[i] = invokeParams[i].ParameterType;
            MethodBuilder = new DynamicMethod("ElfImpl", invokeMethod.ReturnType, invokeParamTypes);
            Gen = MethodBuilder.GetILGenerator();
        }

        private DynamicMethod MethodBuilder { get; }
        protected ILGenerator Gen { get; }

        public T Method
        {
            get
            {
                if (_method != null) return _method;
                EmitEnd();
                _method = (T) MethodBuilder.CreateDelegate(typeof(T));

                return _method;
            }
        }

        public void AddOpcode(ElfOpCode op, int a, int b, int c)
        {
            EmitBeforeOp(a, b, c);
            EmitNumGet(op.A, a);
            EmitNumGet(op.B, b);
            EmitOp(op.Op);
            EmitAfterOp(a, b, c);
        }

        private void EmitNumGet(ElfOpCode.NumGet get, int val)
        {
            switch (get)
            {
                case ElfOpCode.NumGet.Reg:
                    EmitGetReg(val);
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

        protected abstract void EmitAfterOp(int a, int b, int c);
        protected abstract void EmitBeforeOp(int a, int b, int c);
        protected abstract void EmitGetReg(int reg);
        protected abstract void EmitEnd();
    }
}