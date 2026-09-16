# Architecture

The solution separates pure logic (`CorelMate.Core`, `Badges`, `Curves`, `AI`, `Infrastructure`) from the Windows/CorelDRAW boundary (`Host`) and the Docker UI (`UI`). `Host` is the only project referencing the Corel interop assemblies. The verified Docker path is `CorelDRAW.Application.FrameWork.AddDocker` with a public parameterless WPF `UserControl` class from a .NET Framework 4.8 assembly. The developer bootstrap invokes that API through CorelDRAW COM automation; it does not modify CorelDRAW installation files. No feature currently mutates a document.

The release path packages the WPF UI and its managed Host/Badges/Infrastructure dependencies plus session install/remove scripts. CorelDRAW supplies its own interop assemblies. The package is intentionally not presented as an automatic `.addon` startup package because that .NET startup contract is unverified.

The hardened WPF workflow maintains captured-master state, dynamically creates one input column per detected variable plus quantity, recalculates a pure preview, validates rows before mutation, disables mutation controls while generating, and provides Reset without deleting CorelDRAW artwork. COM mutation remains synchronous on the CorelDRAW context.

Convert Text to Curves uses the same materialized `CorelShapeTraversal` in `CorelMate.Host`. `CorelTextToCurvesConverter` performs a no-mutation preflight, then calls CorelDRAW's native `Shape.ConvertToCurves()` for supported selected text inside one command group.