$(function () {
    let fieldCount = 0;
    const fieldTypes = ["text", "email", "textarea", "checkbox", "select"];
    console.log("initialMetadata", initialMetadata);
    if (initialMetadata && initialMetadata !== "{}") {
        let metadata;
        try {
            metadata = initialMetadata;// JSON.parse(initialMetadata);
        } catch (e) {
            console.error("Failed to parse initialMetadata", e);
            return;
        }

        $("#formName").val(metadata.FormName || "");
        console.log("metadata", metadata);
        
        if (Array.isArray(metadata.Fields)) {
            metadata.Fields.forEach(field => {
                addField(field);
            });
        }
    }

    $("#addFieldBtn").click(function () {
        addField();
    });

    $("#saveFormBtn").click(function () {
        const metadata = collectMetadata();
        $("#metadataPreview").text(JSON.stringify(metadata, null, 2));

        // Send to server
        const method = editingFormId > 0 ? "PUT" : "POST";
        const url = editingFormId > 0 ? `/FormBuilder/Update/${editingFormId}` : "/FormBuilder/Save";

        $.ajax({
            url: url,
            type: method,
            contentType: "application/json",
            data: JSON.stringify(metadata),
            success: function () {
                alert("Form saved!");
                location.href = "/";
            },
            error: function () {
                alert("Error saving form.");
            }
        });

    });

    function addField(field = null) {
        const fieldId = `field_${fieldCount++}`;
        const selectedType = field?.type || "text";
        const isSelect = selectedType === "select";
        const optionsVal = field?.options ? field.options.join(",") : "";

        const fieldHtml = `
        <div class="card mb-2 p-3" data-id="${fieldId}">
            <div class="d-flex justify-content-between align-items-center">
                <strong>Field</strong>
                <button class="btn btn-danger btn-sm removeFieldBtn">❌ Remove</button>
            </div>
            <div class="mt-2">
                <label>Label</label>
                <input type="text" class="form-control labelInput" placeholder="Label" value="${field?.label || ''}" />
            </div>
            <div class="mt-2">
                <label>Type</label>
                <select class="form-control typeInput">
                    ${fieldTypes.map(t => `<option value="${t}" ${t === selectedType ? "selected" : ""}>${t}</option>`).join("")}
                </select>
            </div>
            <div class="mt-2">
                <label><input type="checkbox" class="requiredInput" ${field?.required ? "checked" : ""} /> Required</label>
            </div>
            <div class="mt-2 optionsDiv" style="${isSelect ? "" : "display:none"}">
                <label>Options (comma-separated)</label>
                <input type="text" class="form-control optionsInput" value="${optionsVal}" />
            </div>
        </div>
    `;

        $("#fieldList").append(fieldHtml);
    }


    $("#fieldList").on("click", ".removeFieldBtn", function () {
        $(this).closest(".card").remove();
    });

    $("#fieldList").on("change", ".typeInput", function () {
        const type = $(this).val();
        const optionsDiv = $(this).closest(".card").find(".optionsDiv");
        if (type === "select") {
            optionsDiv.show();
        } else {
            optionsDiv.hide();
        }
    });

    function collectMetadata() {
        const formName = $("#formName").val().trim();
        const fields = [];

        $("#fieldList .card").each(function () {
            const label = $(this).find(".labelInput").val().trim();
            const type = $(this).find(".typeInput").val();
            const required = $(this).find(".requiredInput").is(":checked");
            const optionsRaw = $(this).find(".optionsInput").val().trim();

            const field = {
                id: label.toLowerCase().replace(/\s+/g, "_"),
                label: label,
                type: type,
                required: required
            };

            if (type === "select" && optionsRaw) {
                field.options = optionsRaw.split(",").map(o => o.trim());
            }

            fields.push(field);
        });

        return {
            formName: formName,
            fields: fields
        };
    }
});

// Make fieldList sortable
new Sortable(document.getElementById("fieldList"), {
    animation: 150,
    handle: ".card", // drag the whole card
    ghostClass: "bg-light"
});
