choco install -y --no-progress tortoisegit

Write-Output "Waiting for all active installations to finish..."
do {

  $msitest = Get-Process -ProcessName msiexec -ErrorAction SilentlyContinue
  if ($msitest) {
    Start-Sleep -Seconds 60
  }

}
until (!$msitest)
Write-Output "Verified all installations have finished."

#Registry key for sh.exe isn't created until this runs once (causing a failing test)
. "C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe"