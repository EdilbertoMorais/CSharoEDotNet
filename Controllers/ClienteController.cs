// Fiap.Web.Aluno/Controllers/ClienteController.cs
using Fiap.Web.Aluno.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fiap.Web.Aluno.Controllers;

using Data;
using Microsoft.EntityFrameworkCore;

public class ClienteController : Controller {

    private readonly DatabaseContext _databaseContext;

    public ClienteController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public IActionResult Index()
    {
        // Inclui o representante associado a cada cliente para exibição
        var clientes = _databaseContext.Clientes
            .Include(c => c.Representante).ToList();

        // Garante que a lista de clientes não seja nula, inicializando-a se estiver vazia
        if (clientes == null){
            clientes = new List<ClienteModel>();
        }
        return View(clientes);
    }

    // Anotação de uso do verbo HTTP Get para exibir o formulário de criação
    [HttpGet]
    public IActionResult Create()
    {
        Console.WriteLine("Executou a Action Cadastrar()");

        // Carrega os representantes para popular o DropdownList na View
        // Cria um SelectList usando o ID e o Nome do Representante
        var selectListRepresentantes =
            new SelectList(_databaseContext.Representantes.ToList(),
            nameof(RepresentanteModel.RepresentanteId),
            nameof(RepresentanteModel.NomeRepresentante));

        // Adiciona o SelectList à ViewBag para ser acessado na View
        // A propriedade Representantes é criada dinamicamente na ViewBag
        ViewBag.Representantes = selectListRepresentantes;

        // Retorna para a View Create um objeto modelo com as propriedades em branco
        return View(new ClienteModel());
    }

    // Anotação de uso do Verb HTTP Post para processar o formulário de criação
    [HttpPost]
    public IActionResult Create(ClienteModel clienteModel)
    {
        // Verifica se o modelo recebido é válido de acordo com as validações de atributos (DataAnnotations)
        // e se o RepresentanteId informado existe no banco de dados.
        // É importante que essa validação ocorra antes de tentar salvar no banco de dados.
        if (ModelState.IsValid)
        {
            // Tenta encontrar o representante no banco de dados usando o RepresentanteId do clienteModel.
            // Isso verifica se a chave pai existe, prevenindo o erro ORA-02291.
            var representanteExistente = _databaseContext.Representantes.Find(clienteModel.RepresentanteId);

            // Se o representante não for encontrado, significa que o RepresentanteId é inválido.
            if (representanteExistente == null)
            {
                // Adiciona um erro ao ModelState, associado à propriedade RepresentanteId.
                // Esta mensagem será exibida para o usuário na View.
                ModelState.AddModelError("RepresentanteId", "O Representante selecionado não existe.");
            }
            else
            {
                // Se o ModelState for válido (incluindo a validação do RepresentanteId)
                // e o representante existir, adiciona o novo cliente ao contexto e salva as mudanças.
                _databaseContext.Clientes.Add(clienteModel);
                _databaseContext.SaveChanges();

                // Cria uma mensagem de sucesso para ser exibida na próxima requisição (via TempData)
                TempData["mensagemSucesso"] = $"O cliente {clienteModel.Nome} foi cadastrado com sucesso!";

                // Redireciona para a Action Index após o sucesso do cadastro
                return RedirectToAction(nameof(Index));
            }
        }

        // Se o ModelState não for válido (incluindo o erro que adicionamos acima),
        // ou se a validação inicial do modelo falhar, recarrega a lista de representantes
        // para que o DropdownList na View seja preenchido novamente
        // e a View possa ser exibida com as mensagens de erro de validação.
        var selectListRepresentantes =
            new SelectList(_databaseContext.Representantes.ToList(),
            nameof(RepresentanteModel.RepresentanteId),
            nameof(RepresentanteModel.NomeRepresentante));

        ViewBag.Representantes = selectListRepresentantes;

        // Retorna a mesma View com o modelo (clienteModel) e as mensagens de erro
        return View(clienteModel);
    }

    // Anotação de uso do Verb HTTP Get para exibir o formulário de edição
    [HttpGet]
    public IActionResult Edit(int id)
    {
        // Carrega os representantes para popular o DropdownList na View de edição
        var selectListRepresentantes =
            new SelectList(_databaseContext.Representantes.ToList(),
            nameof(RepresentanteModel.RepresentanteId),
            nameof(RepresentanteModel.NomeRepresentante));

        ViewBag.Representantes = selectListRepresentantes;

        // Busca o cliente a ser editado pelo ID
        var clienteConsultado = _databaseContext.Clientes.Find(id);

        // Retorna o cliente consultado para a View de edição
        return View(clienteConsultado);
    }

    // Anotação de uso do Verb HTTP Post para processar o formulário de edição
    [HttpPost]
    public IActionResult Edit(ClienteModel clienteModel)
    {
        // Verifica se o modelo recebido é válido
        if (ModelState.IsValid)
        {
            // Tenta encontrar o representante no banco de dados para validar a chave estrangeira.
            var representanteExistente = _databaseContext.Representantes.Find(clienteModel.RepresentanteId);

            if (representanteExistente == null)
            {
                ModelState.AddModelError("RepresentanteId", "O Representante selecionado não existe.");
            }
            else
            {
                // Se o modelo for válido e o representante existir, atualiza o cliente no contexto e salva as mudanças
                _databaseContext.Clientes.Update(clienteModel);
                _databaseContext.SaveChanges();

                // Cria mensagem de sucesso
                TempData["mensagemSucesso"] = $"Os dados do cliente {clienteModel.Nome} foram alterados com sucesso";
                // Redireciona para a Index
                return RedirectToAction(nameof(Index));
            }
        }

        // Se o ModelState não for válido, recarrega os representantes e retorna a View com o modelo e erros
        var selectListRepresentantes =
            new SelectList(_databaseContext.Representantes.ToList(),
            nameof(RepresentanteModel.RepresentanteId),
            nameof(RepresentanteModel.NomeRepresentante));

        ViewBag.Representantes = selectListRepresentantes;

        return View(clienteModel);
    }

    // Anotação de uso do Verb HTTP Get para exibir detalhes de um cliente
    [HttpGet]
    public IActionResult Detail(int id)
    {
        // Carrega os representantes para popular o DropdownList (se aplicável na view de detalhes)
        var selectListRepresentantes =
            new SelectList(_databaseContext.Representantes.ToList(),
            nameof(RepresentanteModel.RepresentanteId),
            nameof(RepresentanteModel.NomeRepresentante));

        ViewBag.Representantes = selectListRepresentantes;

        // Busca o cliente pelo ID para exibir seus detalhes
        var clienteConsultado = _databaseContext.Clientes.Find(id);
        // Retorna o cliente consultado para a View
        return View(clienteConsultado);
    }


    // Anotação de uso do Verb HTTP Get para deletar um cliente
    [HttpGet]
    public IActionResult Delete(int id)
    {
        // Busca o cliente a ser deletado
        var clienteConsultado = _databaseContext.Clientes.Find(id);
        if (clienteConsultado != null){
            // Se o cliente for encontrado, remove-o do contexto e salva as mudanças
            _databaseContext.Clientes.Remove(clienteConsultado);
            _databaseContext.SaveChanges();

            // Cria mensagem de sucesso
            TempData["mensagemSucesso"] = $"Os dados do cliente {clienteConsultado.Nome} foram removidos com sucesso";
        }
        else{
            // Mensagem de erro se o cliente não for encontrado
            TempData["mensagemSucesso"] = "OPS !!! Cliente inexistente.";
        }
        // Redireciona para a Index após a tentativa de exclusão
        return RedirectToAction(nameof(Index));
    }

}
