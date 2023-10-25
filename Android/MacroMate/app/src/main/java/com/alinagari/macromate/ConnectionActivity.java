package com.alinagari.macromate;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;

import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;

public class ConnectionActivity extends AppCompatActivity {

    private Button btnConnect;
    private EditText etIpAddress;
    private EditText etPort;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_connection);
        btnConnect = findViewById(R.id.btnConnect);
        etIpAddress = findViewById(R.id.etIpAddress);
        etPort = findViewById(R.id.etPortNumber);

        btnConnect.setOnClickListener(view -> {
            SocketManager.setIpNPort(etIpAddress.getText().toString(),
                    Integer.parseInt(etPort.getText().toString()));
            SocketManager.checkConnection(this);

        });
    }
}