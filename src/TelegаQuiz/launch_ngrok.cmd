echo "Install ngrok.exe and add to system PATH."
call ngrok http -host-header="localhost:54875" 54875
pause