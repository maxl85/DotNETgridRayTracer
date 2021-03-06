﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.17929
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Client.GridServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GridServiceReference.IGridService")]
    public interface IGridService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGridService/Connect", ReplyAction="http://tempuri.org/IGridService/ConnectResponse")]
        void Connect(System.Guid agentID);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IGridService/Connect", ReplyAction="http://tempuri.org/IGridService/ConnectResponse")]
        System.IAsyncResult BeginConnect(System.Guid agentID, System.AsyncCallback callback, object asyncState);
        
        void EndConnect(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGridService/Disconnect", ReplyAction="http://tempuri.org/IGridService/DisconnectResponse")]
        void Disconnect(System.Guid agentID);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IGridService/Disconnect", ReplyAction="http://tempuri.org/IGridService/DisconnectResponse")]
        System.IAsyncResult BeginDisconnect(System.Guid agentID, System.AsyncCallback callback, object asyncState);
        
        void EndDisconnect(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGridService/Add", ReplyAction="http://tempuri.org/IGridService/AddResponse")]
        double Add(double a, double b);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IGridService/Add", ReplyAction="http://tempuri.org/IGridService/AddResponse")]
        System.IAsyncResult BeginAdd(double a, double b, System.AsyncCallback callback, object asyncState);
        
        double EndAdd(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGridServiceChannel : Client.GridServiceReference.IGridService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AddCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public AddCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public double Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((double)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GridServiceClient : System.ServiceModel.ClientBase<Client.GridServiceReference.IGridService>, Client.GridServiceReference.IGridService {
        
        private BeginOperationDelegate onBeginConnectDelegate;
        
        private EndOperationDelegate onEndConnectDelegate;
        
        private System.Threading.SendOrPostCallback onConnectCompletedDelegate;
        
        private BeginOperationDelegate onBeginDisconnectDelegate;
        
        private EndOperationDelegate onEndDisconnectDelegate;
        
        private System.Threading.SendOrPostCallback onDisconnectCompletedDelegate;
        
        private BeginOperationDelegate onBeginAddDelegate;
        
        private EndOperationDelegate onEndAddDelegate;
        
        private System.Threading.SendOrPostCallback onAddCompletedDelegate;
        
        public GridServiceClient() {
        }
        
        public GridServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GridServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GridServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GridServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> ConnectCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DisconnectCompleted;
        
        public event System.EventHandler<AddCompletedEventArgs> AddCompleted;
        
        public void Connect(System.Guid agentID) {
            base.Channel.Connect(agentID);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginConnect(System.Guid agentID, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginConnect(agentID, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndConnect(System.IAsyncResult result) {
            base.Channel.EndConnect(result);
        }
        
        private System.IAsyncResult OnBeginConnect(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid agentID = ((System.Guid)(inValues[0]));
            return this.BeginConnect(agentID, callback, asyncState);
        }
        
        private object[] OnEndConnect(System.IAsyncResult result) {
            this.EndConnect(result);
            return null;
        }
        
        private void OnConnectCompleted(object state) {
            if ((this.ConnectCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ConnectCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ConnectAsync(System.Guid agentID) {
            this.ConnectAsync(agentID, null);
        }
        
        public void ConnectAsync(System.Guid agentID, object userState) {
            if ((this.onBeginConnectDelegate == null)) {
                this.onBeginConnectDelegate = new BeginOperationDelegate(this.OnBeginConnect);
            }
            if ((this.onEndConnectDelegate == null)) {
                this.onEndConnectDelegate = new EndOperationDelegate(this.OnEndConnect);
            }
            if ((this.onConnectCompletedDelegate == null)) {
                this.onConnectCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnConnectCompleted);
            }
            base.InvokeAsync(this.onBeginConnectDelegate, new object[] {
                        agentID}, this.onEndConnectDelegate, this.onConnectCompletedDelegate, userState);
        }
        
        public void Disconnect(System.Guid agentID) {
            base.Channel.Disconnect(agentID);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginDisconnect(System.Guid agentID, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginDisconnect(agentID, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndDisconnect(System.IAsyncResult result) {
            base.Channel.EndDisconnect(result);
        }
        
        private System.IAsyncResult OnBeginDisconnect(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid agentID = ((System.Guid)(inValues[0]));
            return this.BeginDisconnect(agentID, callback, asyncState);
        }
        
        private object[] OnEndDisconnect(System.IAsyncResult result) {
            this.EndDisconnect(result);
            return null;
        }
        
        private void OnDisconnectCompleted(object state) {
            if ((this.DisconnectCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.DisconnectCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void DisconnectAsync(System.Guid agentID) {
            this.DisconnectAsync(agentID, null);
        }
        
        public void DisconnectAsync(System.Guid agentID, object userState) {
            if ((this.onBeginDisconnectDelegate == null)) {
                this.onBeginDisconnectDelegate = new BeginOperationDelegate(this.OnBeginDisconnect);
            }
            if ((this.onEndDisconnectDelegate == null)) {
                this.onEndDisconnectDelegate = new EndOperationDelegate(this.OnEndDisconnect);
            }
            if ((this.onDisconnectCompletedDelegate == null)) {
                this.onDisconnectCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnDisconnectCompleted);
            }
            base.InvokeAsync(this.onBeginDisconnectDelegate, new object[] {
                        agentID}, this.onEndDisconnectDelegate, this.onDisconnectCompletedDelegate, userState);
        }
        
        public double Add(double a, double b) {
            return base.Channel.Add(a, b);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginAdd(double a, double b, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginAdd(a, b, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public double EndAdd(System.IAsyncResult result) {
            return base.Channel.EndAdd(result);
        }
        
        private System.IAsyncResult OnBeginAdd(object[] inValues, System.AsyncCallback callback, object asyncState) {
            double a = ((double)(inValues[0]));
            double b = ((double)(inValues[1]));
            return this.BeginAdd(a, b, callback, asyncState);
        }
        
        private object[] OnEndAdd(System.IAsyncResult result) {
            double retVal = this.EndAdd(result);
            return new object[] {
                    retVal};
        }
        
        private void OnAddCompleted(object state) {
            if ((this.AddCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.AddCompleted(this, new AddCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void AddAsync(double a, double b) {
            this.AddAsync(a, b, null);
        }
        
        public void AddAsync(double a, double b, object userState) {
            if ((this.onBeginAddDelegate == null)) {
                this.onBeginAddDelegate = new BeginOperationDelegate(this.OnBeginAdd);
            }
            if ((this.onEndAddDelegate == null)) {
                this.onEndAddDelegate = new EndOperationDelegate(this.OnEndAdd);
            }
            if ((this.onAddCompletedDelegate == null)) {
                this.onAddCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAddCompleted);
            }
            base.InvokeAsync(this.onBeginAddDelegate, new object[] {
                        a,
                        b}, this.onEndAddDelegate, this.onAddCompletedDelegate, userState);
        }
    }
}
