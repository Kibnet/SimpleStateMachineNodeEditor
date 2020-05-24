﻿using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Windows;
using SimpleStateMachineNodeEditor.Helpers.Enums;
using System.Reactive.Linq;
using System.Reactive;
using SimpleStateMachineNodeEditor.ViewModel.NodesCanvas;

namespace SimpleStateMachineNodeEditor.ViewModel
{
    public class ViewModelMainWindow: ReactiveObject
    {
        public ObservableCollectionExtended<ViewModelMessage> Messages { get; set; } = new ObservableCollectionExtended<ViewModelMessage>();

        [Reactive] public ViewModelNodesCanvas NodesCanvas { get; set; }
        [Reactive] public TypeMessage DisplayMessageType { get; set; }
        [Reactive] public bool? DebugEnable { get; set; } = true;

        private IDisposable ConnectToMessages;
        public double MaxHeightMessagePanel = 150;



        public ViewModelMainWindow(ViewModelNodesCanvas viewModelNodesCanvas)
        {
            NodesCanvas = viewModelNodesCanvas;
            SetupCommands();
            SetupSubscriptions();        
        }

        #region Setup Subscriptions

        private void SetupSubscriptions()
        {
           this.WhenAnyValue(x => x.NodesCanvas.DisplayMessageType).Subscribe(_ => UpdateMessages());

        }
        private void UpdateMessages()
        {
            ConnectToMessages?.Dispose();
            Messages.Clear();

            bool debugEnable = DebugEnable.HasValue && DebugEnable.Value;
            bool displayAll = this.NodesCanvas.DisplayMessageType == TypeMessage.All;

            ConnectToMessages = this.NodesCanvas.Messages.ToObservableChangeSet().Filter(x=> CheckForDisplay(x.TypeMessage)).ObserveOnDispatcher().Bind(Messages).DisposeMany().Subscribe();

            bool CheckForDisplay(TypeMessage typeMessage)
            {
                if (typeMessage == this.NodesCanvas.DisplayMessageType)
                {
                    return true;
                }
                else if(typeMessage==TypeMessage.Debug)
                {
                    return debugEnable && displayAll;
                }

                return displayAll;
            }
        }
        #endregion Setup Subscriptions
        #region Setup Commands

        public ReactiveCommand<string, Unit> CommandCopyError { get; set; }

        private void SetupCommands()
        {
            CommandCopyError = ReactiveCommand.Create<string>(CopyError);

        }

        private void CopyError(string errrorText)
        {
            Clipboard.SetText(errrorText);
        }

        #endregion Setup Commands

    }
}
