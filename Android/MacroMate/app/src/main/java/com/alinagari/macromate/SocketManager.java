package com.alinagari.macromate;

import android.content.Context;
import android.content.Intent;

import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;

public class SocketManager {
    public static String SERVER_IP;
    public static int SERVER_PORT;

    public static void setIpNPort(String ip, int port) {
        SERVER_IP = ip;
        SERVER_PORT = port;
    }

    public static void checkConnection(Context context) {
        new Thread(() -> {
            try {
                Socket socket = new Socket(SERVER_IP, SERVER_PORT);
                DataOutputStream dataOutputStream = new DataOutputStream(socket.getOutputStream());
                dataOutputStream.writeUTF("Android Here!");
                dataOutputStream.close();
                socket.close();
                context.startActivity(new Intent(context, MainActivity.class));
            } catch (IOException e) {
                e.printStackTrace();
            }
        }).start();
    }

    public static void runMacro(String macro) {
        new Thread(() -> {
            try {
                Socket socket = new Socket(SERVER_IP, SERVER_PORT);
                DataOutputStream dataOutputStream = new DataOutputStream(socket.getOutputStream());
                dataOutputStream.writeUTF(macro);
                dataOutputStream.close();
                socket.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }).start();
    }
}
