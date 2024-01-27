package com.alinagari.macromate

import android.graphics.BitmapFactory
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.ImageView
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.io.DataInputStream
import java.io.IOException
import java.net.Socket

class DataActivity : AppCompatActivity() {
    private var IP: String? = null
    private var PORT: Int = 0
    private var imgs = arrayOfNulls<ImageView>(10)
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_data)
        IP = intent.getStringExtra("IP")
        PORT = intent.getIntExtra("PORT", 0)
        socketMessage("connected")

        var btn = findViewById<Button>(R.id.btnGet)
        imgs = Array(10) { ImageView(this) }
        imgs[0] = findViewById(R.id.img1)
        imgs[1] = findViewById(R.id.img2)
        imgs[2] = findViewById(R.id.img3)
        imgs[3] = findViewById(R.id.img4)
        imgs[4] = findViewById(R.id.img5)
        imgs[5] = findViewById(R.id.img6)
        imgs[6] = findViewById(R.id.img7)
        imgs[7] = findViewById(R.id.img8)
        imgs[8] = findViewById(R.id.img9)
        imgs[9] = findViewById(R.id.img10)

        btn.setOnClickListener {
            CoroutineScope(Dispatchers.IO).launch {
                socketConnect()
            }
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

    /*
        private fun socketConnect() {
            Log.d("SocketConnect", "sip $IP $PORT")
            try {
                Socket(IP, PORT).use { socket ->
                    DataInputStream(socket.getInputStream()).use { inputStream ->
                        Thread.sleep(200)
                        for (i in 0..9) {
                            val dataSize = 5000
                            val imgData = ByteArray(dataSize)
                            inputStream.read(imgData, 0, dataSize)
                            try {
                                val image = BitmapFactory.decodeByteArray(imgData, 0, imgData.size)
                                if (image != null) {
                                    runOnUiThread {
                                        imgs[i]?.setImageBitmap(image)
                                    }
                                } else {
                                    Log.e("ImageUpdate", "Failed to decode image. ${imgData.size}")
                                }
                            } catch (e: Exception) {
                                Log.e(
                                    "ImageUpdate",
                                    "Error updating image: ${e.message}  ${imgData.size}"
                                )
                            }
                            Thread.sleep(100)
                        }
                        while (true) {
                            val endSignal = ByteArray(3)
                            inputStream.read(endSignal, 0, 3)
                            if(String(endSignal, Charsets.UTF_8) == "END") {
                                runOnUiThread{Toast.makeText(applicationContext, "end", Toast.LENGTH_SHORT).show()}
                                Thread.sleep(1000);
                                break
                            }
                        }
                    }
                }
            } catch (e: IOException) {
                e.printStackTrace()
                Log.w("Exception", e.localizedMessage)
            }
        }
    */
    private fun socketConnect() {
        Log.d("SocketConnect", "sip $IP $PORT")
        try {
            Socket(IP, PORT).use { socket ->
                DataInputStream(socket.getInputStream()).use { inputStream ->
                    Thread.sleep(200)
                    for (i in 0..9) {
                        val intra = ByteArray(4)
                        inputStream.read(intra, 0, 4)
                        val dataSize = String(intra, Charsets.UTF_8).toInt()
                        val imgData = ByteArray(dataSize)
                        inputStream.read(imgData, 0, dataSize)
                        try {
                            val image = BitmapFactory.decodeByteArray(imgData, 0, imgData.size)
                            if (image != null) {
                                runOnUiThread {
                                    imgs[i]?.setImageBitmap(image)
                                }
                            } else {
                                Log.e("ImageUpdate", "Failed to decode image. ${imgData.size}")
                            }
                        } catch (e: Exception) {
                            Log.e(
                                "ImageUpdate",
                                "Error updating image: ${e.message}  ${imgData.size}"
                            )
                        }
                        Thread.sleep(100)
                    }
                }
            }
        } catch (e: IOException) {
            e.printStackTrace()
            Log.w("Exception", e.localizedMessage)
        }
    }

}