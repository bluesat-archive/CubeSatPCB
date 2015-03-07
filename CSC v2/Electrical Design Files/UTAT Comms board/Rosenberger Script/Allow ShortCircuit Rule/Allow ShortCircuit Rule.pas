Procedure CreateShortCircuitRule;
var
   PCBBoard                      :  IPCB_Board;
   Rule                          :  IPCB_ShortCircuitConstraint;


Begin
     PCBBoard := PCBServer.GetCurrentPCBBoard;
     If PCBBoard = Nil Then Exit;

     Rule := PCBServer.PCBRuleFactory(eRule_ShortCircuit);

     Rule.Scope1Expression := 'IsBoardCutoutRegion and ' +
                              '(HasFootprint(''ROS-119S202-40ML5_V'') or ' +
                              'HasFootprint(''ROS-119S242-40ML5_V'') or ' +
                              'HasFootprint(''ROS-18S203-40ML5_V'') or ' +
                              'HasFootprint(''ROS-18S243-40ML5_V'') or ' +
                              'HasFootprint(''ROS-19S202-40ML5_V'') or ' +
                              'HasFootprint(''ROS-19S242-40ML5_V'') or ' +
                              'HasFootprint(''ROS-28K203-40ML5_V'') or ' +
                              'HasFootprint(''ROS-28K209-40ML5_V'') or ' +
                              'HasFootprint(''ROS-32K145-400L5_V'') or ' +
                              'HasFootprint(''ROS-32K242-40ML5_V''))' ;

     Rule.Scope2Expression := 'IsVia OR IsPad';
     Rule.Name := 'ShortCircuit_BoardCutout';
     Rule.Enabled := TRUE;
     Rule.Allowed := TRUE;

     PCBBoard.AddPCBObject(Rule);
End;
