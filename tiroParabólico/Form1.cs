using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace tiroParabólico
{
    public partial class FirstInter : Form
    {
        private bool arrastrando = false;
        private Point ubicacionInicialPictureBox;
        private Point puntoInicioArrastre;
        private Point posicionOriginal;

        // Variables para el movimiento parabolico
        private Timer timerMovimiento;
        private double velocidadX, velocidadY;
        private double gravedad = 0.5;
        private int rebotesRestantes = 10;
        private int lineaSueloY;
        private bool enMovimiento = false;
        private Point puntoDestino;

        // Variables para calculos fisicos
        private double velocidadInicial;
        private double anguloLanzamiento;
        private double angulomaxalt;
        private double angulomaxaltcol;
        private DateTime tiempoInicioVuelo;
        private double alturaMaxima;
        private double alturaMacimaCol;
        private double desplazamientoMaximo;
        private Point posicionInicialVuelo;
        private bool alturaMaximaAlcanzada = false;

        private bool chartsActivation = false;

        // Random para posicion aleatoria
        private Random random = new Random();

        // Variables para datos de los chars
        private List<double> tiempos = new List<double>();
        private List<double> posicionesVertical = new List<double>();
        private List<double> posicionesHorizontal = new List<double>();
        private List<double> velocidadesHorizontal = new List<double>();
        private List<double> velocidadesVertical = new List<double>();
        private List<double> magnitudesVelocidad = new List<double>();
        private List<double> angulosVelocidad = new List<double>();

        public FirstInter()
        {
            InitializeComponent();
            ConfigurarArrastrePictureBox();
            ConfigurarMovimientoParabolico();
            ConfigurarCharts();
            panelGraficas.Visible = false;
        }

        private void ConfigurarCharts()
        {
            // Posicion Vertical vs Tiempo
            ConfigurarChart(chart1, "Posición Vertical vs Tiempo", "Tiempo (s)", "Posición Y (px)");

            // Posicion Horizontal vs Tiempo
            ConfigurarChart(chart2, "Posición Horizontal vs Tiempo", "Tiempo (s)", "Posición X (px)");

            // Trayectoria (X vs Y)
            ConfigurarChart(chart3, "Trayectoria", "Posición X (px)", "Posición Y (px)");

            // Velocidad Horizontal vs Tiempo
            ConfigurarChart(chart4, "Velocidad Horizontal vs Tiempo", "Tiempo (s)", "Velocidad X (px/frame)");

            // Velocidad Vertical vs Tiempo
            ConfigurarChart(chart5, "Velocidad Vertical vs Tiempo", "Tiempo (s)", "Velocidad Y (px/frame)");

            // Magnitud de Velocidad vs Tiempo
            ConfigurarChart(chart6, "Magnitud de Velocidad vs Tiempo", "Tiempo (s)", "Velocidad (px/frame)");

            // angulo de Velocidad vs Tiempo
            ConfigurarChart(chart7, "Ángulo de Velocidad vs Tiempo", "Tiempo (s)", "Ángulo (grados)");
        }

        private void ConfigurarChart(Chart chart, string titulo, string ejeX, string ejeY)
        {
            chart.Series.Clear();
            chart.ChartAreas.Clear();

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            chart.Titles.Clear();
            chart.Titles.Add(titulo);

            chartArea.AxisX.Title = ejeX;
            chartArea.AxisY.Title = ejeY;

            Series series = new Series();
            series.ChartType = SeriesChartType.Line;
            series.BorderWidth = 2;
            series.Color = Color.Blue;
            chart.Series.Add(series);

            // Configurar grid
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
        }

        private void LimpiarDatosCharts()
        {
            tiempos.Clear();
            posicionesVertical.Clear();
            posicionesHorizontal.Clear();
            velocidadesHorizontal.Clear();
            velocidadesVertical.Clear();
            magnitudesVelocidad.Clear();
            angulosVelocidad.Clear();
        }

        private void ActualizarCharts()
        {
            // Limpiar series
            foreach (Chart chart in new Chart[] { chart1, chart2, chart3, chart4, chart5, chart6, chart7 })
            {
                if (chart.Series.Count > 0)
                    chart.Series[0].Points.Clear();
            }

            // Chart 1: Posición Vertical vs Tiempo
            for (int i = 0; i < tiempos.Count; i++)
            {
                chart1.Series[0].Points.AddXY(tiempos[i], posicionesVertical[i]);
            }

            // Chart 2: Posición Horizontal vs Tiempo
            for (int i = 0; i < tiempos.Count; i++)
            {
                chart2.Series[0].Points.AddXY(tiempos[i], posicionesHorizontal[i]);
            }

            // Chart 3: Trayectoria (X vs Y)
            for (int i = 0; i < posicionesHorizontal.Count; i++)
            {
                chart3.Series[0].Points.AddXY(posicionesHorizontal[i], posicionesVertical[i]);
            }

            // Chart 4: Velocidad Horizontal vs Tiempo
            for (int i = 0; i < tiempos.Count; i++)
            {
                chart4.Series[0].Points.AddXY(tiempos[i], velocidadesHorizontal[i]);
            }

            // Chart 5: Velocidad Vertical vs Tiempo
            for (int i = 0; i < tiempos.Count; i++)
            {
                chart5.Series[0].Points.AddXY(tiempos[i], velocidadesVertical[i]);
            }

            // Chart 6: Magnitud de Velocidad vs Tiempo
            for (int i = 0; i < tiempos.Count; i++)
            {
                chart6.Series[0].Points.AddXY(tiempos[i], magnitudesVelocidad[i]);
            }

            // Chart 7: Ángulo de Velocidad vs Tiempo
            for (int i = 0; i < tiempos.Count; i++)
            {
                chart7.Series[0].Points.AddXY(tiempos[i], angulosVelocidad[i]);
            }

            // Refrescar todos los charts
            foreach (Chart chart in new Chart[] { chart1, chart2, chart3, chart4, chart5, chart6, chart7 })
            {
                chart.Invalidate();
            }
        }

        private void FirstInter_Load(object sender, EventArgs e)
        {
            posicionOriginal = huevo.Location;
            lineaSueloY = this.ClientSize.Height - 20;
            PosicionarSartenAleatorio();
        }

        private void PosicionarSartenAleatorio()
        {
            int margen = 20;
            int areaDisponibleX = this.ClientSize.Width - sarten.Width - (margen * 2);
            int areaDisponibleY = this.ClientSize.Height - sarten.Height - (margen * 2);

            if (areaDisponibleX > 0 && areaDisponibleY > 0)
            {
                int posX = random.Next(margen, margen + areaDisponibleX);
                int posY = random.Next(margen, margen + areaDisponibleY);
                sarten.Location = new Point(posX, posY);
            }
            else
            {
                sarten.Location = new Point(
                    (this.ClientSize.Width - sarten.Width) / 2,
                    (this.ClientSize.Height - sarten.Height) / 2
                );
            }
        }

        private bool HayColision()
        {
            Rectangle rectHuevo = new Rectangle(huevo.Location, huevo.Size);
            Rectangle rectSarten = new Rectangle(sarten.Location, sarten.Size);
            return rectHuevo.IntersectsWith(rectSarten);
        }

        private void ManejarColision()
        {
            timerMovimiento.Stop();
            enMovimiento = false;
            huevo.Location = posicionOriginal;
            PosicionarSartenAleatorio();
            MostrarEfectoColision();
        }

        private void MostrarEfectoColision()
        {
            Color colorOriginal = this.BackColor;
            this.BackColor = Color.LightGreen;
            Timer timerEfecto = new Timer();
            timerEfecto.Interval = 200;
            timerEfecto.Tick += (s, e) =>
            {
                this.BackColor = colorOriginal;
                timerEfecto.Stop();
                timerEfecto.Dispose();
            };
            timerEfecto.Start();
        }

        public void ReposicionarSarten()
        {
            PosicionarSartenAleatorio();
        }

        private void ConfigurarArrastrePictureBox()
        {
            huevo.MouseDown += PictureBox_MouseDown;
            huevo.MouseMove += PictureBox_MouseMove;
            huevo.MouseUp += PictureBox_MouseUp;
            huevo.Cursor = Cursors.Hand;
        }

        private void ConfigurarMovimientoParabolico()
        {
            timerMovimiento = new Timer();
            timerMovimiento.Interval = 16;
            timerMovimiento.Tick += TimerMovimiento_Tick;
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !enMovimiento)
            {
                arrastrando = true;
                puntoInicioArrastre = e.Location;
                ubicacionInicialPictureBox = huevo.Location;
                huevo.BringToFront();
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastrando && !enMovimiento)
            {
                int nuevaX = huevo.Left + (e.X - puntoInicioArrastre.X);
                int nuevaY = huevo.Top + (e.Y - puntoInicioArrastre.Y);

                nuevaX = Math.Max(0, Math.Min(nuevaX, this.ClientSize.Width - huevo.Width));
                nuevaY = Math.Max(0, Math.Min(nuevaY, this.ClientSize.Height - huevo.Height));

                huevo.Location = new Point(nuevaX, nuevaY);
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (arrastrando && !enMovimiento)
            {
                arrastrando = false;
                puntoDestino = huevo.Location;
                IniciarMovimientoParabolico();
            }
        }

        private void IniciarMovimientoParabolico()
        {
            // Limpiar datos anteriores
            LimpiarDatosCharts();

            // Calcular el vector desde la posición original al punto de destino
            int deltaX = puntoDestino.X - posicionOriginal.X;
            int deltaY = puntoDestino.Y - posicionOriginal.Y;

            // Invertir la dirección (movimiento contrario al arrastre)
            deltaX = -deltaX;
            deltaY = -deltaY;

            // Calcular la distancia total (hipotenusa del triángulo)
            double distancia = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // Normalizar y escalar la velocidad
            double factorVelocidad = 0.2;

            if (distancia > 0)
            {
                velocidadX = (deltaX / distancia) * distancia * factorVelocidad;
                velocidadY = (deltaY / distancia) * distancia * factorVelocidad;
            }
            else
            {
                velocidadX = 0;
                velocidadY = -5;
            }

            // Iniciar variables
            CalcularVariablesFisicasIniciales();

            rebotesRestantes = 2;
            enMovimiento = true;
            alturaMaximaAlcanzada = false;
            tiempoInicioVuelo = DateTime.Now;
            posicionInicialVuelo = huevo.Location;

            timerMovimiento.Start();

            ImprimirDatosIniciales();
        }

        private void CalcularVariablesFisicasIniciales()
        {
            // Calcular velocidad inicial
            velocidadInicial = Math.Sqrt(velocidadX * velocidadX + velocidadY * velocidadY);

            // Calcular ángulo de lanzamiento (en grados)
            anguloLanzamiento = Math.Atan2(velocidadY, velocidadX) * (180 / Math.PI);

            // Reiniciar mediciones
            alturaMaxima = 0;
            desplazamientoMaximo = 0;
        }

        private void ImprimirDatosIniciales()
        {
            velocidadinic.Text = velocidadInicial.ToString();
            velocidadinicx.Text = velocidadX.ToString();
            velocidadinicy.Text = (velocidadY * -1).ToString();
            velocidadinicangulo.Text = (anguloLanzamiento * -1).ToString();
        }

        private void TimerMovimiento_Tick(object sender, EventArgs e)
        {
            if (!enMovimiento) return;

            // Calcular tiempo transcurrido
            TimeSpan tiempoVuelo = DateTime.Now - tiempoInicioVuelo;
            double tiempoSegundos = tiempoVuelo.TotalSeconds;

            // Aplicar gravedad
            double velocidadYAnterior = velocidadY;
            velocidadY += gravedad;

            // Calcular nueva posicion
            int nuevaX = huevo.Left + (int)velocidadX;
            int nuevaY = huevo.Top + (int)velocidadY;

            // Guardar datos
            GuardarDatosParaCharts(tiempoSegundos, nuevaX, nuevaY);

            // Actualizacion de mediciones
            ActualizarMediciones(tiempoSegundos, nuevaY);

            // Se verifica la colision entre objetos
            Point posicionTemporal = new Point(nuevaX, nuevaY);
            if (VerificarColisionEnPosicion(posicionTemporal))
            {
                if (rebotesRestantes > 0)
                {
                    ImprimirDatosRebote();

                    // Reducir la velocdad en un 40%
                    velocidadY = -velocidadY * 0.6;
                    velocidadX = velocidadX * 0.6;

                    // Ajustar la posición para que el proyectil rebote sobre el objetivo y no se hunda
                    nuevaY = sarten.Top - huevo.Height;

                    rebotesRestantes--;
                }
                else
                {
                    // Si ya no quedan rebotes, detener la simulación
                    enMovimiento = false;
                    timerMovimiento.Stop();
                    huevo.Location = new Point(nuevaX, sarten.Top - huevo.Height);
                    ImprimirDatosFinales("COLISIÓN");
                    ActualizarCharts();

                    panelFinal.Visible = true;
                    return;
                }
            }

            // 2. VERIFICAR COLISIÓN CON EL SUELO
            if (nuevaY + huevo.Height >= lineaSueloY)
            {
                if (rebotesRestantes > 0)
                {
                    ImprimirDatosRebote();

                    // Reducir la velocidad en un 40% (queda el 60%)
                    velocidadY = -velocidadY * 0.6;
                    velocidadX = velocidadX * 0.6;

                    nuevaY = lineaSueloY - huevo.Height;
                    rebotesRestantes--;
                }
                else
                {
                    // Detener al alcanzar el último impacto en el suelo
                    enMovimiento = false;
                    timerMovimiento.Stop();
                    huevo.Location = new Point(nuevaX, lineaSueloY - huevo.Height);
                    ImprimirDatosFinales("COLISIÓN");
                    ActualizarCharts();

                    panelFinal.Visible = true;
                    return;
                }
            }

            // 3. VERIFICAR LÍMITES LATERALES
            if (nuevaX <= 0 || nuevaX >= this.ClientSize.Width - huevo.Width)
            {
                velocidadX = -velocidadX * 0.6;
                nuevaX = Math.Max(0, Math.Min(nuevaX, this.ClientSize.Width - huevo.Width));
            }

            // Verificar si sale por arriba
            if (nuevaY < 0)
            {
                nuevaY = 0;
                velocidadY = Math.Abs(velocidadY) * 0.5;
            }

            huevo.Location = new Point(nuevaX, nuevaY);
            this.Invalidate();
        }

        private void GuardarDatosParaCharts(double tiempo, int posX, int posY)
        {
            // Guardar tiempo
            tiempos.Add(tiempo);

            // Guardar posiciones (Y invertida para que sea más intuitiva en el gráfico)
            posicionesHorizontal.Add(posX);
            posicionesVertical.Add(this.ClientSize.Height - posY); // Invertir Y

            // Guardar velocidades
            velocidadesHorizontal.Add(velocidadX);
            velocidadesVertical.Add(-velocidadY); // Invertir Y para que positivo sea hacia arriba

            // Calcular y guardar magnitud de velocidad
            double magnitud = Math.Sqrt(velocidadX * velocidadX + velocidadY * velocidadY);
            magnitudesVelocidad.Add(magnitud);

            // Calcular y guardar ángulo de velocidad
            double angulo = Math.Atan2(-velocidadY, velocidadX) * (180.0 / Math.PI);
            angulosVelocidad.Add(angulo);
        }

        private void ActualizarMediciones(double tiempoSegundos, int nuevaY)
        {
            // Calcular altura actual (invertida porque Y aumenta hacia abajo)
            double alturaActual = posicionInicialVuelo.Y - nuevaY;

            // Actualizar altura máxima
            if (alturaActual > alturaMaxima)
            {
                alturaMaxima = alturaActual;
                alturaMacimaCol = alturaActual;
            }

            // Detectar cuando se alcanza la altura máxima (cuando Vy cambia de signo)
            if (velocidadY > 0 && !alturaMaximaAlcanzada)
            {
                alturaMaximaAlcanzada = true;
                ImprimirDatosAlturaMaxima(tiempoSegundos);
            }

            // Actualizar desplazamiento máximo
            double desplazamientoActual = Math.Abs(huevo.Left - posicionInicialVuelo.X);
            if (desplazamientoActual > desplazamientoMaximo)
            {
                desplazamientoMaximo = desplazamientoActual;
            }
        }

        private void ImprimirDatosAlturaMaxima(double tiempoSegundos)
        {
            double velocidadEnAlturaMaxima = Math.Abs(velocidadX); // Vy = 0 en altura máxima
            double velocidadActual = Math.Sqrt(velocidadX * velocidadX + velocidadY * velocidadY);

            // Calcular ángulo (en grados)
            angulomaxalt = Math.Atan2(velocidadY, velocidadX) * (180 / Math.PI);

            alturamax.Text = alturaMaxima.ToString();
            velocidadmaxalt.Text = velocidadEnAlturaMaxima.ToString();
            velocidadmaxaltx.Text = velocidadX.ToString();
            velocidadmaxalty.Text = velocidadY.ToString();
            velocidadmaxaltangulo.Text = angulomaxalt.ToString();
        }

        private void ImprimirDatosRebote()
        {
            double velocidadActual = Math.Sqrt(velocidadX * velocidadX + velocidadY * velocidadY);
        }

        private void ImprimirDatosFinales(string motivo)
        {
            TimeSpan tiempoTotal = DateTime.Now - tiempoInicioVuelo;
            double velocidadFinal = Math.Sqrt(velocidadX * velocidadX + velocidadY * velocidadY);

            tiempodevuelo.Text = tiempoTotal.TotalSeconds.ToString();
            horizontalmax.Text = desplazamientoMaximo.ToString();

            angulomaxaltcol = Math.Atan2(velocidadY, velocidadX) * (180 / Math.PI);

            // Cálculos teoricos adicionales
            if (motivo == "COLISIÓN")
            {
                velocidadfinal.Text = velocidadFinal.ToString();
                velocidadfinalx.Text = velocidadX.ToString();
                velocidadfinaly.Text = (velocidadY * -1).ToString();
                velocidadfinalangulo.Text = (angulomaxaltcol * -1).ToString();

                alturamax.Text = alturaMacimaCol.ToString();

                if (alturaMaximaAlcanzada == false)
                {
                    velocidadmaxaltangulo.Text = (angulomaxaltcol * -1).ToString();
                    velocidadmaxaltx.Text = velocidadX.ToString();
                    velocidadmaxalty.Text = (velocidadY * -1).ToString();
                    velocidadmaxalt.Text = velocidadFinal.ToString();
                }
            }

            if (motivo == "FIN POR REBOTES")
            {
                velocidadfinal.Text = "";
                velocidadfinalx.Text = "";
                velocidadfinaly.Text = "";
                velocidadfinalangulo.Text = "";
            }
        }

        private bool VerificarColisionEnPosicion(Point posicion)
        {
            Rectangle rectHuevo = new Rectangle(posicion, huevo.Size);
            Rectangle rectSarten= new Rectangle(sarten.Location, sarten.Size);
            return rectHuevo.IntersectsWith(rectSarten);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen penSuelo = new Pen(Color.Transparent, 2))
            {
                e.Graphics.DrawLine(penSuelo, 0, lineaSueloY, this.ClientSize.Width, lineaSueloY);
            }
        }

        private void AnimarRegreso(Point destino)
        {
            if (enMovimiento) return;

            Timer timerAnimacion = new Timer();
            timerAnimacion.Interval = 10;
            int pasos = 40;
            int pasoActual = 0;

            Point inicio = huevo.Location;
            double incrementoX = (destino.X - inicio.X) / (double)pasos;
            double incrementoY = (destino.Y - inicio.Y) / (double)pasos;

            timerAnimacion.Tick += (s, args) =>
            {
                if (pasoActual < pasos)
                {
                    int x = inicio.X + (int)(incrementoX * pasoActual);
                    int y = inicio.Y + (int)(incrementoY * pasoActual);
                    huevo.Location = new Point(x, y);

                    if (HayColision())
                    {
                        ManejarColision();
                        timerAnimacion.Stop();
                        timerAnimacion.Dispose();
                    }
                    pasoActual++;
                }
                else
                {
                    huevo.Location = destino;
                    timerAnimacion.Stop();
                    timerAnimacion.Dispose();
                }
            };
            timerAnimacion.Start();
        }

        public void ResetearTiro()
        {
            timerMovimiento.Stop();
            enMovimiento = false;
            huevo.Location = posicionOriginal;
            rebotesRestantes = 2;
            LimpiarDatosCharts();

            // Limpiar todos los charts
            foreach (Chart chart in new Chart[] { chart1, chart2, chart3, chart4, chart5, chart6, chart7 })
            {
                if (chart.Series.Count > 0)
                    chart.Series[0].Points.Clear();
                chart.Invalidate();
            }

            this.Invalidate();
        }


        private void spaceship_Click(object sender, EventArgs e)
        {
            ReposicionarSarten();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.R)
            {
                ResetearTiro();
            }
            else if (e.KeyCode == Keys.S)
            {
                ReposicionarSarten();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Right)
            {
                ResetearTiro();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            lineaSueloY = this.ClientSize.Height - 20;
            if (sarten != null)
            {
                if (sarten.Right > this.ClientSize.Width || sarten.Bottom > this.ClientSize.Height)
                {
                    ReposicionarSarten();
                }
            }
        }

        private void charts_Click(object sender, EventArgs e)
        {
            panelGraficas.Visible = true;
            panelFinal.Visible = false;
            huevo.Visible = false;
            sarten.Visible = false;
            canasta.Visible = false;
            
        }

        private void btnVolverD_Click(object sender, EventArgs e)
        {
            panelMostrarDatos.Visible = false;
            panelFinal.Visible=true;
            huevo.Visible = true;
            sarten.Visible = true;
            canasta.Visible = true;
        }

        private void btnMostrarGaficasD_Click(object sender, EventArgs e)
        {
            panelGraficas.Visible=true;
            panelMostrarDatos.Visible = false;
            huevo.Visible = false;
            sarten.Visible = false;
            canasta.Visible = false;
        }

        private void btnMostrarDatosG_Click(object sender, EventArgs e)
        {
            panelGraficas.Visible=false;
            panelMostrarDatos.Visible=true;
            huevo.Visible = false;
            sarten.Visible = false;
            canasta.Visible = false;
        }

        private void btnMostrarDatos_Click(object sender, EventArgs e)
        {
            panelMostrarDatos.Visible = true;
            panelFinal.Visible = false;
            huevo.Visible = false;
            sarten.Visible = false;
            canasta.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panelFinal.Visible = false;
            huevo.Visible = true;
            sarten.Visible = true;
            canasta.Visible = true;
            AnimarRegreso(posicionOriginal);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            panelGraficas.Visible=false;
            panelFinal.Visible = true;
            huevo.Visible = true;
            sarten.Visible = true;
            canasta.Visible = true;
        }
    }
}