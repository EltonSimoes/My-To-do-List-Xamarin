using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Content;
using Android.Views;

namespace AppToDoList
{
    [Activity(Label = "To-Do List Tarefas", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        //Uma lista para manter os itens da lista
        //Esta lista para armazenar os dados
        public List<string> Itens { get; set;}

        //Este adaptador é usado para conectar dados ao listview
        ArrayAdapter<string> adapter;

        //Configure o arquivo de preferências compartilhadas onde todos os itens serão armazenados
        ISharedPreferences prefs = Application.Context.GetSharedPreferences("TODO_DATA", FileCreationMode.Private);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Defina a visão a partir do recurso de layout "principal"
            SetContentView(Resource.Layout.Main);

            //Inicializa a Lista
            Itens = new List<string>();

            //Carregar qualquer item de lista existente Preferências compartilhadas
            LoadList();

            //Adicione a lista da lista
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemMultipleChoice, Itens);
            ListAdapter = adapter;

            //Clicando no botão Adicionar
            Button addButton = FindViewById<Button>(Resource.Id.AddButton);
            addButton.Click += delegate
            {
                //clique no código do botão

                //Pegue caixa EditText e retire o item de string
                EditText itemText = FindViewById<EditText>(Resource.Id.itemText);
                string item = itemText.Text;

                //Certifique-se de que tem um item não nulo para adicionar
                if (item == "" || item == null)
                {
                    //Este é um item em branco, basta retornar
                    return;
                }

                //Adicione este novo item à principal lista de itens
                Itens.Add(item);

                //Adicione este novo item à lista de adaptadores
                adapter.Add(item);

                //Deixe o listview saber que a lista do adaptador mudou
                adapter.NotifyDataSetChanged();

                //Limpe a caixa de texto para nova entrada
                itemText.Text = "";

                //Atualiza os pares de chave / valor armazenados em preferências compartilhadas
                UpdatedStoredData();
            };
        }//Fim de onCreate

        //Este é o método que é disparado quando um item da lista está marcado
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);

            // Quando o usuário clicar na caixa de seleção, queremos remover todos os itens marcados
            // Da lista como "feito" e remova as prefs compartilhadas
            // Dê uma verificação de alerta de confirmação primeiro
            RunOnUiThread(() =>
            {
                AlertDialog.Builder builder;
                builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirma");
                builder.SetMessage("Você terminou com esta tarefa?");
                builder.SetCancelable(true);

                builder.SetPositiveButton("OK", delegate
                {
                    // Remove o item da lista e da lista Itens
                    var item = Itens[position];
                    Itens.Remove(item);
                    adapter.Remove(item);

                    // redefina o listview para que nada seja selecionado
                    l.ClearChoices();
                    l.RequestLayout();

                    // atualiza os dados armazenados nas preferências compartilhadas
                    UpdatedStoredData();
                });

                builder.SetNegativeButton("Cancel", delegate
                {
                    return;
                });

                // isso lança o popup "modal"
                builder.Show();

            });
        }
        // este método carrega nos itens que estão em preferências compartilhadas
        // e preenche a lista

        public void LoadList()
        {
            // primeiro precisamos descobrir quantas horas temos em preferências compartilhadas
            // use a tecla itemCount para descobrir
            int count = prefs.GetInt("itemCount", 0);

            // fica perto da quantidade de itens que deve ter
            // quando obtemos cada par de chaves / valores em SP, adicione-os à Lista de Itens
            if (count > 0)
            {
                Toast.MakeText(this, "Getting saved items...", Android.Widget.ToastLength.Short).Show();

                for (int i = 0; i < count; i++)
                {
                    string item = prefs.GetString(i.ToString(), null);
                    if (item != null)
                    {
                        Itens.Add(item);
                    }
                }
            }
        }// fim da lista de carga
            //this method updates the stored key/value pairs we holding in shared preferences
            public void UpdatedStoredData()
            {
            // remove os itens atuais nas preferências compartilhadas
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Clear();
            editor.Commit();

            // adiciona todos os itens da lista às preferências compartilhadas
            // então, se o aplicativo estiver fechado, podemos reabrir a lista
            editor = prefs.Edit();

            // chave que faz o controle de quantos itens armazenamos no SP
            editor.PutInt("itemCount", Itens.Count);

            int counter = 0;
            // faça um loop em cada item da lista e adicione-o às preferências compartilhadas
            // lista para ser escrita            
            foreach (string item in Itens)
            {
                editor.PutString(counter.ToString(), item);
                counter++;
            }

            // escreve para o SP
            editor.Apply();

        }
    }
}

