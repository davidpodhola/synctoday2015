//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#if !EXCLUDE_CODEGEN
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998

namespace sync.today.orleans.interfaces
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using System.Collections.Generic;
    using Orleans;
    using Orleans.Runtime;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.0.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    public class LogPageBreakFactory
    {
        

                        public static sync.today.orleans.interfaces.ILogPageBreak GetGrain(long primaryKey)
                        {
                            return Cast(global::Orleans.CodeGeneration.GrainFactoryBase.MakeGrainReferenceInternal(typeof(sync.today.orleans.interfaces.ILogPageBreak), 1741168979, primaryKey));
                        }

                        public static sync.today.orleans.interfaces.ILogPageBreak GetGrain(long primaryKey, string grainClassNamePrefix)
                        {
                            return Cast(global::Orleans.CodeGeneration.GrainFactoryBase.MakeGrainReferenceInternal(typeof(sync.today.orleans.interfaces.ILogPageBreak), 1741168979, primaryKey, grainClassNamePrefix));
                        }

            public static sync.today.orleans.interfaces.ILogPageBreak Cast(global::Orleans.Runtime.IAddressable grainRef)
            {
                
                return LogPageBreakReference.Cast(grainRef);
            }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.0.0")]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
        [System.SerializableAttribute()]
        [global::Orleans.CodeGeneration.GrainReferenceAttribute("sync.today.orleans.interfaces.sync.today.orleans.interfaces.ILogPageBreak")]
        internal class LogPageBreakReference : global::Orleans.Runtime.GrainReference, global::Orleans.Runtime.IAddressable, sync.today.orleans.interfaces.ILogPageBreak
        {
            

            public static sync.today.orleans.interfaces.ILogPageBreak Cast(global::Orleans.Runtime.IAddressable grainRef)
            {
                
                return (sync.today.orleans.interfaces.ILogPageBreak) global::Orleans.Runtime.GrainReference.CastInternal(typeof(sync.today.orleans.interfaces.ILogPageBreak), (global::Orleans.Runtime.GrainReference gr) => { return new LogPageBreakReference(gr);}, grainRef, 1741168979);
            }
            
            protected internal LogPageBreakReference(global::Orleans.Runtime.GrainReference reference) : 
                    base(reference)
            {
            }
            
            protected internal LogPageBreakReference(SerializationInfo info, StreamingContext context) : 
                    base(info, context)
            {
            }
            
            protected override int InterfaceId
            {
                get
                {
                    return 1741168979;
                }
            }
            
            public override string InterfaceName
            {
                get
                {
                    return "sync.today.orleans.interfaces.sync.today.orleans.interfaces.ILogPageBreak";
                }
            }
            
            [global::Orleans.CodeGeneration.CopierMethodAttribute()]
            public static object _Copier(object original)
            {
                LogPageBreakReference input = ((LogPageBreakReference)(original));
                return ((LogPageBreakReference)(global::Orleans.Runtime.GrainReference.CopyGrainReference(input)));
            }
            
            [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
            public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
            {
                LogPageBreakReference input = ((LogPageBreakReference)(original));
                global::Orleans.Runtime.GrainReference.SerializeGrainReference(input, stream, expected);
            }
            
            [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
            public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
            {
                return LogPageBreakReference.Cast(((global::Orleans.Runtime.GrainReference)(global::Orleans.Runtime.GrainReference.DeserializeGrainReference(expected, stream))));
            }
            
            public override bool IsCompatible(int interfaceId)
            {
                return ((interfaceId == this.InterfaceId) 
                            || (interfaceId == 1928988877));
            }
            
            protected override string GetMethodName(int interfaceId, int methodId)
            {
                return LogPageBreakMethodInvoker.GetMethodName(interfaceId, methodId);
            }
            
            System.Threading.Tasks.Task sync.today.orleans.interfaces.ILogPageBreak.Log()
            {

                return base.InvokeMethodAsync<object>(351394857, new object[] {} );
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.0.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [global::Orleans.CodeGeneration.MethodInvokerAttribute("sync.today.orleans.interfaces.sync.today.orleans.interfaces.ILogPageBreak", 1741168979)]
    internal class LogPageBreakMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        
        int global::Orleans.CodeGeneration.IGrainMethodInvoker.InterfaceId
        {
            get
            {
                return 1741168979;
            }
        }
        
        global::System.Threading.Tasks.Task<object> global::Orleans.CodeGeneration.IGrainMethodInvoker.Invoke(global::Orleans.Runtime.IAddressable grain, int interfaceId, int methodId, object[] arguments)
        {

            try
            {                    if (grain == null) throw new System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case 1741168979:  // ILogPageBreak
                        switch (methodId)
                        {
                            case 351394857: 
                                return ((ILogPageBreak)grain).Log().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                        }case 1928988877:  // IGrainWithIntegerKey
                        switch (methodId)
                        {
                            default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                        }
                    default:
                        throw new System.InvalidCastException("interfaceId="+interfaceId);
                }
            }
            catch(Exception ex)
            {
                var t = new System.Threading.Tasks.TaskCompletionSource<object>();
                t.SetException(ex);
                return t.Task;
            }
        }
        
        public static string GetMethodName(int interfaceId, int methodId)
        {

            switch (interfaceId)
            {
                
                case 1741168979:  // ILogPageBreak
                    switch (methodId)
                    {
                        case 351394857:
                            return "Log";
                    
                        default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                    }
                case 1928988877:  // IGrainWithIntegerKey
                    switch (methodId)
                    {
                        
                        default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                    }

                default:
                    throw new System.InvalidCastException("interfaceId="+interfaceId);
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.0.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    public class StartWorkflowFactory
    {
        

                        public static sync.today.orleans.interfaces.IStartWorkflow GetGrain(long primaryKey)
                        {
                            return Cast(global::Orleans.CodeGeneration.GrainFactoryBase.MakeGrainReferenceInternal(typeof(sync.today.orleans.interfaces.IStartWorkflow), 1391960172, primaryKey));
                        }

                        public static sync.today.orleans.interfaces.IStartWorkflow GetGrain(long primaryKey, string grainClassNamePrefix)
                        {
                            return Cast(global::Orleans.CodeGeneration.GrainFactoryBase.MakeGrainReferenceInternal(typeof(sync.today.orleans.interfaces.IStartWorkflow), 1391960172, primaryKey, grainClassNamePrefix));
                        }

            public static sync.today.orleans.interfaces.IStartWorkflow Cast(global::Orleans.Runtime.IAddressable grainRef)
            {
                
                return StartWorkflowReference.Cast(grainRef);
            }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.0.0")]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
        [System.SerializableAttribute()]
        [global::Orleans.CodeGeneration.GrainReferenceAttribute("sync.today.orleans.interfaces.sync.today.orleans.interfaces.IStartWorkflow")]
        internal class StartWorkflowReference : global::Orleans.Runtime.GrainReference, global::Orleans.Runtime.IAddressable, sync.today.orleans.interfaces.IStartWorkflow
        {
            

            public static sync.today.orleans.interfaces.IStartWorkflow Cast(global::Orleans.Runtime.IAddressable grainRef)
            {
                
                return (sync.today.orleans.interfaces.IStartWorkflow) global::Orleans.Runtime.GrainReference.CastInternal(typeof(sync.today.orleans.interfaces.IStartWorkflow), (global::Orleans.Runtime.GrainReference gr) => { return new StartWorkflowReference(gr);}, grainRef, 1391960172);
            }
            
            protected internal StartWorkflowReference(global::Orleans.Runtime.GrainReference reference) : 
                    base(reference)
            {
            }
            
            protected internal StartWorkflowReference(SerializationInfo info, StreamingContext context) : 
                    base(info, context)
            {
            }
            
            protected override int InterfaceId
            {
                get
                {
                    return 1391960172;
                }
            }
            
            public override string InterfaceName
            {
                get
                {
                    return "sync.today.orleans.interfaces.sync.today.orleans.interfaces.IStartWorkflow";
                }
            }
            
            [global::Orleans.CodeGeneration.CopierMethodAttribute()]
            public static object _Copier(object original)
            {
                StartWorkflowReference input = ((StartWorkflowReference)(original));
                return ((StartWorkflowReference)(global::Orleans.Runtime.GrainReference.CopyGrainReference(input)));
            }
            
            [global::Orleans.CodeGeneration.SerializerMethodAttribute()]
            public static void _Serializer(object original, global::Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
            {
                StartWorkflowReference input = ((StartWorkflowReference)(original));
                global::Orleans.Runtime.GrainReference.SerializeGrainReference(input, stream, expected);
            }
            
            [global::Orleans.CodeGeneration.DeserializerMethodAttribute()]
            public static object _Deserializer(System.Type expected, global::Orleans.Serialization.BinaryTokenStreamReader stream)
            {
                return StartWorkflowReference.Cast(((global::Orleans.Runtime.GrainReference)(global::Orleans.Runtime.GrainReference.DeserializeGrainReference(expected, stream))));
            }
            
            public override bool IsCompatible(int interfaceId)
            {
                return ((interfaceId == this.InterfaceId) 
                            || (interfaceId == 1928988877));
            }
            
            protected override string GetMethodName(int interfaceId, int methodId)
            {
                return StartWorkflowMethodInvoker.GetMethodName(interfaceId, methodId);
            }
            
            System.Threading.Tasks.Task sync.today.orleans.interfaces.IStartWorkflow.Start()
            {

                return base.InvokeMethodAsync<object>(1420312199, new object[] {} );
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.0.0")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [global::Orleans.CodeGeneration.MethodInvokerAttribute("sync.today.orleans.interfaces.sync.today.orleans.interfaces.IStartWorkflow", 1391960172)]
    internal class StartWorkflowMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        
        int global::Orleans.CodeGeneration.IGrainMethodInvoker.InterfaceId
        {
            get
            {
                return 1391960172;
            }
        }
        
        global::System.Threading.Tasks.Task<object> global::Orleans.CodeGeneration.IGrainMethodInvoker.Invoke(global::Orleans.Runtime.IAddressable grain, int interfaceId, int methodId, object[] arguments)
        {

            try
            {                    if (grain == null) throw new System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case 1391960172:  // IStartWorkflow
                        switch (methodId)
                        {
                            case 1420312199: 
                                return ((IStartWorkflow)grain).Start().ContinueWith(t => {if (t.Status == System.Threading.Tasks.TaskStatus.Faulted) throw t.Exception; return (object)null; });
                            default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                        }case 1928988877:  // IGrainWithIntegerKey
                        switch (methodId)
                        {
                            default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                        }
                    default:
                        throw new System.InvalidCastException("interfaceId="+interfaceId);
                }
            }
            catch(Exception ex)
            {
                var t = new System.Threading.Tasks.TaskCompletionSource<object>();
                t.SetException(ex);
                return t.Task;
            }
        }
        
        public static string GetMethodName(int interfaceId, int methodId)
        {

            switch (interfaceId)
            {
                
                case 1391960172:  // IStartWorkflow
                    switch (methodId)
                    {
                        case 1420312199:
                            return "Start";
                    
                        default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                    }
                case 1928988877:  // IGrainWithIntegerKey
                    switch (methodId)
                    {
                        
                        default: 
                            throw new NotImplementedException("interfaceId="+interfaceId+",methodId="+methodId);
                    }

                default:
                    throw new System.InvalidCastException("interfaceId="+interfaceId);
            }
        }
    }
}
#pragma warning restore 162
#pragma warning restore 219
#pragma warning restore 693
#pragma warning restore 1591
#pragma warning restore 1998
#endif
