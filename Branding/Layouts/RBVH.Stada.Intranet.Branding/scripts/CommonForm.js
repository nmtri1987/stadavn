var Functions =
    {
        getSPFieldRender: function (ctx, fieldName) {
            var fieldContext = ctx;
            var result = ctx.ListSchema.Field.filter(function (obj) {
                return obj.Name === fieldName;
            });
            fieldContext.CurrentFieldSchema = result[0];
            fieldContext.CurrentFieldValue = ctx.ListData.Items[0][fieldName];
            return ctx.Templates.Fields[fieldName](fieldContext);
        },
        getSPFieldTitle: function (ctx, fieldName) {
            var result = ctx.ListSchema.Field.filter(function (obj) {
                return obj.Name === fieldName;
            });
            return result[0].Title;
        }
    }









