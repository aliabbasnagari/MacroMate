package com.alinagari.macromate

import android.content.Intent
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Base64
import android.util.Log
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.Toast
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.io.ByteArrayOutputStream
import java.io.DataInputStream
import java.io.IOException
import java.net.Socket

class DataActivity : AppCompatActivity() {
    private var IP: String? = null
    private var PORT: Int = 0
    private var llout: LinearLayout? = null
    val bitmaps = mutableListOf<Bitmap?>()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_data)
        IP = intent.getStringExtra("IP")
        PORT = intent.getIntExtra("PORT", 0)
        socketMessage("connected")

        llout = findViewById(R.id.llout)

        var btn = findViewById<Button>(R.id.btnGet)
        btn.setOnClickListener {
            Toast.makeText(this, bitmaps.size.toString(), Toast.LENGTH_SHORT).show();
            for (img in bitmaps) {
                val imageView = ImageView(this)
                imageView.setImageBitmap(img)
                llout?.addView(imageView)
            }
            val rr = receiveImages()
        }

    }

    private fun socketMessage(message: String) {
        Log.d("socketMessage", "sip $IP $PORT")
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val socket = Socket(IP, PORT)
                val dataOutputStream = socket.getOutputStream()
                dataOutputStream.write(message.toByteArray())
                dataOutputStream.close()
                socket.close()
            } catch (e: IOException) {
                e.printStackTrace()
                e.localizedMessage?.let { Log.w("Exception", it) }
            }
        }
    }
    private fun BitmapToString(bitmap: Bitmap): String {
        val byteArrayOutputStream = ByteArrayOutputStream()
        bitmap.compress(Bitmap.CompressFormat.PNG, 100, byteArrayOutputStream)
        val byteArray = byteArrayOutputStream.toByteArray()
        return Base64.encodeToString(byteArray, Base64.DEFAULT)
    }

    private fun receiveImages() {
        CoroutineScope(Dispatchers.IO).launch {
            val endMarker =
                "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<END_MARKER>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>";
            try {
                val socket = Socket(IP, PORT)
                val inputStream = socket.getInputStream()
                val dataInputStream = DataInputStream(inputStream)
                var buffer = ByteArray(70)
                var bytesRead: Int

                var byteArrayOutputStream = ByteArrayOutputStream()
                while (true) {
                    bytesRead = dataInputStream.read(buffer, 0, buffer.size)
                    if (bytesRead == -1) break

                    if (String(buffer.copyOf(bytesRead), Charsets.UTF_8).contains(endMarker)) {
                        val byteArray = byteArrayOutputStream.toByteArray()
                        try {
                            val bitmap = BitmapFactory.decodeByteArray(byteArray, 0, byteArray.size)
                            bitmaps.add(bitmap)
                            Log.d(
                                "DONE",
                                "${byteArray.size}, ${if (bitmap == null) "NULL" else "IMAGE"}"
                            )
                        } catch (ex: Exception) {
                            ex.localizedMessage?.let { Log.e("EXC", it) }
                        } finally {
                            byteArrayOutputStream = ByteArrayOutputStream()
                            buffer = ByteArray(70)
                        }
                    } else {
                        byteArrayOutputStream.write(buffer, 0, bytesRead)
                    }
                }

                byteArrayOutputStream.close()
                dataInputStream.close()
                socket.close()
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }
}