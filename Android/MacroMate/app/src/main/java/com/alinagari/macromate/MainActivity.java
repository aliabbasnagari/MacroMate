package com.alinagari.macromate;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;

public class MainActivity extends AppCompatActivity {

    private Button[] panelBtn = new Button[6];

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        View decorView = getWindow().getDecorView();
        int uiOptions = View.SYSTEM_UI_FLAG_HIDE_NAVIGATION | View.SYSTEM_UI_FLAG_FULLSCREEN;
        decorView.setSystemUiVisibility(uiOptions);


        panelBtn[0] = findViewById(R.id.button1);
        panelBtn[1] = findViewById(R.id.button2);
        panelBtn[2] = findViewById(R.id.button3);
        panelBtn[3] = findViewById(R.id.button4);
        panelBtn[4] = findViewById(R.id.button5);
        panelBtn[5] = findViewById(R.id.button6);

        panelBtn[0].setOnClickListener(view -> {
            SocketManager.runMacro("1");
        });

        panelBtn[1].setOnClickListener(view -> {
            SocketManager.runMacro("2");
        });

        panelBtn[2].setOnClickListener(view -> {
            SocketManager.runMacro("3");
        });

        panelBtn[3].setOnClickListener(view -> {
            SocketManager.runMacro("4");
        });

        panelBtn[4].setOnClickListener(view -> {
            SocketManager.runMacro("5");
        });

        panelBtn[5].setOnClickListener(view -> {
            SocketManager.runMacro("6");
        });
    }

    @Override
    protected void onStop() {
        SocketManager.runMacro("exit");
        finish();
        super.onStop();
    }

    @Override
    protected void onDestroy() {
        SocketManager.runMacro("exit");
        super.onDestroy();
    }
}