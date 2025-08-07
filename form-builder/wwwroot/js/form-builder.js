$(function () {
    let fieldCount = 0;
    const fieldTypes = ["text", "email", "textarea", "checkbox", "select"];

    $("#addFieldBtn").click(function () {
        addField();
    });

    $("#saveFormBtn").click(function () {
        const metadata = collectMetadata();
        $("#metadataPreview").text(JSON.stringify(metadata, null, 2));

        // Send to server
        $.ajax({
            url: "/FormBuilder/Save",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(metadata),
            success: function () {
                alert("Form saved!");
                location.href = "/"; // or redirect to list
            },
            error: function () {
                alert("Error saving form.");
            }
        });
    });

    function addField() {
        const fieldId = `field_${fieldCount++}`;
        const fieldHtml = `
            <div class="card mb-2 p-3" data-id="${fieldId}">
                <div class="d-flex justify-content-between align-items-center">
                    <strong>Field</strong>
                    <button class="btn btn-danger btn-sm removeFieldBtn">❌ Remove</button>
                </div>
                <div class="mt-2">
                    <label>Label</label>
                    <input type="text" class="form-control labelInput" placeholder="Label" />
                </div>
                <div class="mt-2">
                    <label>Type</label>
                    <select class="form-control typeInput">
                        ${fieldTypes.map(t => `<option value="${t}">${t}</option>`).join("")}
                    </select>
                </div>
                <div class="mt-2">
                    <label><input type="checkbox" class="requiredInput" /> Required</label>
                </div>
                <div class="mt-2 optionsDiv" style="display: none;">
                    <label>Options (comma-separated)</label>
                    <input type="text" class="form-control optionsInput" placeholder="Option1,Option2" />
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
