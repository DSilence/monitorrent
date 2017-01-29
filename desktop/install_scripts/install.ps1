$arguments = "/I", "$PSScriptRoot\\Monitorrent.msi", "/passive", "/qn"
(Start-Process msiexec -ArgumentList $arguments -Wait -Passthru -verb RunAs).ExitCode