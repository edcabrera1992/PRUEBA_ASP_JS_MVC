var _id = 0;
let _modoEdicion = false;
const API_URL = '/api/Venta';
const API_URL_CLIENTE = '/api/Cliente';
//const API_URL = 'http://localhost:5002/api/venta'; 
var fechaVentaInput, montoTotalInput, clienteIdInput, montoInput, montoTotalLabel = '';
let listaPagos = []; // Array para almacenar los pagos


document.addEventListener("DOMContentLoaded", function () {
    cargarVentas();
    cargarClientes();
});
function CargarDatos() {

    fechaVentaInput = document.getElementById('fechaVenta').value;
    montoTotalInput = document.getElementById('montoTotal').value;
    clienteIdInput = document.getElementById('clienteId').value;
    montoInput = parseFloat(document.getElementById('montoPago').value);
    montoTotalLabel = document.getElementById('montoTotalLabel');
 
}
function abrirModal() {
    CargarDatos();
    console.log('montoTotalInput', montoTotalInput);
    montoTotalLabel.textContent = 'Monto total: '+montoTotalInput;
}
function VaciarDatos() {

    fechaVentaInput.value = '';
    montoTotalInput.value = '';
    clienteIdInput.value = '';
    _id = 0;
    document.getElementById('montoPago').value = '';
}
function cargarVentas() {
    fetch(API_URL)
        .then(response => response.json())
        .then(data => {
            if (data.status !== "success") {
                console.error("Error en la API:", data.response);
                return;
            }

            const ventas = data.response;
            const tablaVentas = document.querySelector('#tablaVentas tbody');
            tablaVentas.innerHTML = '';

            ventas.forEach(venta => {

                const fecha = new Date(venta.fechaVenta);
                const fechaISO = `${fecha.getFullYear()}-${(fecha.getMonth() + 1).toString().padStart(2, '0')}-${fecha.getDate().toString().padStart(2, '0')}`;

                console.log('fechaISO', fechaISO);
                const fila = document.createElement('tr');
                fila.innerHTML = `
                    <td>${venta.id}</td>
                    <td>${new Date(venta.fechaVenta).toLocaleDateString()}</td>
                    <td>${venta.montoTotal.toFixed(2)}</td>
                    <td style="display:none">${venta.clienteId}</td>
                    <td>${venta.nombre}</td>
                    <td>
                      <button onclick="mostrarFormulario(${venta.id}, '${fechaISO}', ${venta.montoTotal.toFixed(2)}, ${venta.clienteId})">Editar</button>

                        <button onclick="eliminarVenta(${venta.id})">Eliminar</button>
                    </td>
                `;
                tablaVentas.appendChild(fila);
            });
        })
        .catch(error => console.error('Error al cargar las ventas:', error));
}
function mostrarFormulario(id = null, fechaISO = '', montoTotal = '', clienteId = '') {


    const formulario = document.getElementById('formularioVenta');

    _id = id;
    document.getElementById('fechaVenta').value = fechaISO;
    document.getElementById('montoTotal').value = montoTotal;
    document.getElementById('clienteId').value = clienteId;

    formulario.style.display = 'block';

    const titulo = document.getElementById('tituloFormulario');
    const btnGuardar = document.getElementById('btnGuardar');

    if (id) {
        _modoEdicion = true;
        _id = id;
        titulo.textContent = 'Editar Venta';

    } else {
        _modoEdicion = false;
        _id = 0;
        titulo.textContent = 'Agregar Nueva Venta';

    }
}

function ocultarFormulario() {
    document.getElementById('formularioVenta').style.display = 'none';
}
function validar() {
  

    if (_id) {

        actualizarVenta();
    } else {

        guardarVenta();
    }

}

function guardarVenta() {

    CargarDatos();
    const tablaPagos = document.getElementById('tablaPagos').getElementsByTagName('tbody')[0];
    if (tablaPagos.rows.length === 0) {
        alert("Debe agregar al menos un pago antes de guardar la venta.");
        return;  // Detener la ejecución de la función si no hay pagos
    }

    const venta = {
        FechaVenta: fechaVentaInput,
        MontoTotal: montoTotalInput,
        ClienteId: clienteIdInput,
        pagos: listaPagos // Enviar la lista de pagos
        
    };

    console.log('montoInput::::', montoInput);

    fetch(API_URL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(venta)
    })
        .then(response => response.json())
        .then(() => {
            VaciarDatos();
            listaPagos = [];
            document.getElementById('tablaPagos').getElementsByTagName('tbody')[0].innerHTML = '';
            document.getElementById('totalMonto').textContent = '0';

            ocultarFormulario();
            cargarVentas();
        })
        .catch(error => console.error('Error al guardar la venta:', error));
}

function actualizarVenta() {
    CargarDatos();
    const venta = {
        FechaVenta: fechaVentaInput,
        MontoTotal: montoTotalInput,
        ClienteId: clienteIdInput
    };
    console.log('_id::', _id);
    fetch(`${API_URL}/${_id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(venta, montoInput)
    })
        .then(response => response.json())
        .then(() => {


            _id = 0;
            ocultarFormulario();
            cargarVentas();
        })
        .catch(error => console.error('Error al actualizar la venta:', error));
}

function eliminarVenta(id) {
    if (confirm('¿Estás seguro de que deseas eliminar esta venta?')) {
        fetch(`${API_URL}/${id}`, { method: 'DELETE' })
            .then(() => cargarVentas())
            .catch(error => console.error('Error al eliminar la venta:', error));
    }
}

function cargarClientes() {
    fetch(`${API_URL_CLIENTE}`)
        .then(response => response.json())
        .then(data => {
            const clienteSelect = document.getElementById('clienteId');
            clienteSelect.innerHTML = '<option value="">Seleccione un cliente</option>';

            data.response.forEach(cliente => {
                let option = document.createElement('option');
                option.value = cliente.id;
                option.textContent = cliente.nombre;
                clienteSelect.appendChild(option);
            });
        })
        .catch(error => console.error('Error al cargar clientes:', error));

}


function realizarPago() {
    try {
        // Capturar el valor del input de monto
        const montoInput = document.getElementById('montoPago').value;

        // Validar si el monto está vacío, no es un número o es menor o igual a 0
        if (!montoInput || isNaN(montoInput) || parseFloat(montoInput) <= 0) {
            throw new Error("Por favor ingrese un monto válido.");
        }

        // Aquí puedes agregar el código para procesar el pago
        console.log(`Pago realizado por: Bs. ${montoInput}`);

        // Cerrar el modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('pagoModal'));
        if (modal) {
            modal.hide();
        }

        // Limpiar el campo de entrada después de cerrar el modal


        return "success";
    } catch (error) {
        alert(error.message); // Mostrar el error en un alert
        return error.message; // Devolver el mensaje de error
    }
}

function agregarPago() {
    try {
    // Obtener los valores de los campos
    const montoPago = parseFloat(document.getElementById('montoPago').value);
    const tipoPago = document.getElementById('tipoPago').value;

  

    // Crear el objeto de pago
    const pago = {
        MontoPago: montoPago,
        TipoPago: tipoPago
    };

    // Agregar el pago a la lista
    listaPagos.push(pago);

    // Agregar los datos a la tabla
    const tablaPagos = document.getElementById('tablaPagos').getElementsByTagName('tbody')[0];
    const nuevaFila = tablaPagos.insertRow();

    // Agregar celdas a la fila
    const celdaTipoPago = nuevaFila.insertCell(0);
    celdaTipoPago.textContent = tipoPago;

    const celdaMonto = nuevaFila.insertCell(1);
    celdaMonto.textContent = montoPago.toFixed(2);

    // Agregar el botón de eliminar
    const celdaEliminar = nuevaFila.insertCell(2);
    const btnEliminar = document.createElement("button");
    btnEliminar.textContent = "Eliminar";
    btnEliminar.classList.add("btn", "btn-danger");
    btnEliminar.onclick = function () {
        eliminarPago(montoPago, nuevaFila);
    };
    celdaEliminar.appendChild(btnEliminar);

    // Actualizar la suma de los montos
    actualizarTotalMonto(montoPago);

        return "success";
    } catch (error) {
        alert(error.message); // Mostrar el error en un alert
        return error.message; // Devolver el mensaje de error
    }

}


function eliminarPago(montoPago, fila) {
    // Eliminar el pago de la lista
    const index = listaPagos.findIndex(pago => pago.MontoPago === montoPago);
    if (index > -1) {
        listaPagos.splice(index, 1);
    }

    // Eliminar la fila de la tabla
    fila.remove();

    // Actualizar la suma de los montos
    actualizarTotalMonto(-montoPago);
}

function actualizarTotalMonto(monto) {
    // Actualizar el monto total en el footer
    const totalMonto = document.getElementById('totalMonto');
    const totalActual = parseFloat(totalMonto.textContent) || 0;
    totalMonto.textContent = (totalActual + monto).toFixed(2);
}



function Reporte() {
    // Realiza una solicitud GET al endpoint que genera el reporte PDF
    fetch(`${API_URL}/reporte-pdf` )  // Cambia la URL según la ruta de tu API
        .then(response => {
            if (!response.ok) {
                throw new Error('Error al generar el reporte');
            }
            return response.blob();  // Lee la respuesta como un Blob (binario)
        })
        .then(blob => {
            // Crear un enlace para descargar el archivo PDF
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'Reporte_Ventas.pdf';  // El nombre del archivo descargado
            document.body.appendChild(a);  // Agregar el enlace al DOM
            a.click();  // Simula un clic para descargar el archivo
            document.body.removeChild(a);  // Eliminar el enlace del DOM
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Hubo un problema al generar el reporte');
        });
}
