import pyautogui
import socket

# Define the server's IP address and port
SERVER_IP = '192.168.18.221'  # Listen on all available network interfaces
SERVER_PORT = 8080  # Use an available port number

# Create a socket object
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the server address and port
server_socket.bind((SERVER_IP, SERVER_PORT))

# Listen for incoming connections
server_socket.listen(5)  # Maximum number of queued connections

print(f"Server is listening on {SERVER_IP}:{SERVER_PORT}")

while True:
    # Accept a connection from a client
    client_socket, client_address = server_socket.accept()

    print(f"Connection from {client_address}")

    # Receive and print data from the client
    data = ""
    data = client_socket.recv(1024).decode('utf-8').strip()
    print(f"Received data: {data}")
    data_cleaned = ''.join(char for char in data if char.isprintable())

    if data_cleaned == '1':
        pyautogui.press("1")
    elif data_cleaned == '2':
        pyautogui.press("2")
    elif data_cleaned == '3':
        pyautogui.press("3")
    elif data_cleaned == '4':
        pyautogui.press("4")
    elif data_cleaned == '5':
        pyautogui.press("5")
    elif data_cleaned == '6':
        pyautogui.press("6")
    elif data_cleaned == 'exit':
        break;

    # Optionally, you can process the data here.

    # Close the client socket
    client_socket.close()