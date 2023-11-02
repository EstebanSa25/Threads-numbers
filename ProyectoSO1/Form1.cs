namespace ProyectoSO1
{
    using Guna.UI2.WinForms;
    using Microsoft.VisualBasic;
    using System.Diagnostics.Metrics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;
    public partial class Form1 : Form
    {
        public readonly int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };//Arreglo de numeros
        List<int> even_numbers = new List<int>();//Listas para guardar las potencias de los numeros pares
        List<int> odd_numbers = new List<int>();//Listas para guardar las factoriales de los numeros impares
        Thread thread_even;//Declaracion de hilo par
        Thread thread_odd;//Declaracion de hilo impar
        public Form1()
        {
            InitializeComponent();
        }
        #region Functions
        public void startThread(Thread thread) //Funcion generica que inicia o muestra un hilo
        {
            if (!thread.IsAlive)// Validacion para saber si el hilo no se ha iniciado
            {
                thread.Start();// Comienza a ejecutar el hilo
            }
        }
        public void updateLabel(Guna2HtmlLabel lbl, string valor)//Funcion generica que actualiza un label
        {
            Action update = () => lbl.Text = valor;//Funcion lambda que actualiza el label lbl.
            Invoke(update);//Invocacion de la accion para actualizar el label
        }
        public void genericCalculation(bool odd, Guna2DataGridView data)//Funcion generica que se ejecuta en los hilos
        {
            List<int> lista = odd ? odd_numbers : even_numbers;// validacion ternaria para saber si se ejecuta el hilo en modo en par o impar y guarda la lista dependiendo
            foreach (var number in numbers)
            {
                updateLabel(lblProcesandoNumero, $"Procesando:{number}");/*Actualiza el label del numero*/
                if (odd && !isEven(number))//Validacion para saber si el numero es impar o par
                {
                    updateLabel(lblTipo, "Tipo: impar");//Actualiza el label del tipo
                    lista.Add(Calculate_Factorial(number));//Agrega el factorial a la lista
                    updateLabel(lblHilo, $"Hilo: {thread_odd.ManagedThreadId}");//Actualiza el label del hilo
                }
                if (!odd && isEven(number))//Validacion para saber si es impar o par
                {
                    updateLabel(lblTipo, "Tipo: par");//Actualiza el label del tipo
                    lista.Add(Calculate_Power(number));//Agrega la potencia a la lista
                    updateLabel(lblHilo, $"Hilo: {thread_even.ManagedThreadId}");//Actualiza el label del hilo
                }
                Thread.Sleep(1000);//Espera 1 segundo
                bpHilos.Value = number;//Actualiza la barra de progreso
            }
            number_filling(data, lista);//Llena el datagridview con la lista
        }
        public void number_filling(Guna2DataGridView L, List<int> numbers)//Funcion generica que llena los valores en un datagridview
        {
            int counter = 0; //Contador para saber en que numero va
            L.Invoke(new Action(() => //Invocacion de la accion para actualizar el datagridview
            {
                foreach (var number in numbers)//Recorre la lista de numeros
                {
                    L.Rows.Add();//Agrega una fila
                    L.Rows[counter].Cells[0].Value = number;//Agrega el numero a la fila
                    counter++;//Aumenta el contador
                }
            }));
        }
        private int Calculate_Factorial(double number)//Funcion que calcula el factorial de un numero
        {
            int factorial = 1;//Inicializa el factorial
            for (int i = 1; i <= number; i++)//Recorre los numeros hasta el numero que se le pasa
            {
                factorial *= i;//Calcula el factorial
            }
            return factorial;//Retorna el factorial
        }
        public bool isEven(int number) => number % 2 == 0; //Funcion lambda que valida si un numero es par o impar
        public int Calculate_Power(int number) => (int)Math.Pow(number, 2); //Funcion lambda que calcula la potencia de un numero
        #endregion
        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            number_filling(dgNumero, numbers.ToList());//Llena el datagridview con los numeros
            thread_even = new Thread(new ThreadStart(() => genericCalculation(false, dgPotencia))); // Llamada al método generico para el hilo par
            thread_odd = new Thread(new ThreadStart(() => genericCalculation(true, dgFactorial))); // Llamada al método generico para el hilo impar
            startThread(thread_even); // Llamada al método para el hilo par
            startThread(thread_odd); // Llamada al método para el hilo impar
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
