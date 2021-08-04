function showCategoryChart() {
    $.get("https://localhost:44350/Products/getCategoryStatistics").then((res) => {
        let data = {}
        console.log(res)
        for (var cat = 0; cat < res.categoryNames.length; cat++) {
            let count = 0;
            for (prod of res.products) {
                for (prodCat of prod.category) {
                    if (prodCat.categoryName === res.categoryNames[cat]) {
                        count++;
                    }
                 }
            }

            data[res.categoryNames[cat]] = count;
            console.log(count);
        }

        var width = 450
        height = 450
        margin = 40
        var radius = Math.min(width, height) / 2 - margin


        var svg = d3.select("#CategoryStats")
            .append("svg")
            .attr("width", width)
            .attr("height", height)
            .append("g")
            .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");


        var color = d3.scaleOrdinal()
            .domain(data)
            .range(d3.schemeAccent);

        var pie = d3.pie()
            .value(function (d) { return d.value; })
        var data_ready = pie(d3.entries(data))

        var arcGenerator = d3.arc()
            .innerRadius(0)
            .outerRadius(radius)

        svg
            .selectAll('mySlices')
            .data(data_ready)
            .enter()
            .append('path')
            .attr('d', arcGenerator)
            .attr('fill', function (d) { console.log(d); return(color(d.data.key)) })
            .attr("stroke", "black")
            .style("stroke-width", "2px")
            .style("opacity", 0.7)

        // Now add the annotation. Use the centroid method to get the best coordinates
        svg
            .selectAll('mySlices')
            .data(data_ready)
            .enter()
            .append('text')
            .text(function (d) { return d.data.key + ": " + d.data.value })
            .attr("transform", function (d) { return "translate(" + arcGenerator.centroid(d) + ")"; })
            .style("text-anchor", "middle")
            .style("font-size", 17)
    })
}


function showUserPermissionCharts() {
    $.get("https://localhost:44350/Users/getUserStatistics").then((response) => {


        // set the dimensions and margins of the graph
        var margin = { top: 30, right: 30, bottom: 70, left: 60 },
            width = 460 - margin.left - margin.right,
            height = 400 - margin.top - margin.bottom;

        // append the svg object to the body of the page
        var svg = d3.select("#UsersStats")
            .append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform",
                "translate(" + margin.left + "," + margin.top + ")");

        var data = response.map(item => ({ permission: item.permission, count: item.count }))

        // X axis
        var x = d3.scaleBand()
            .range([0, width])
            .domain(data.map(function (d) { return d.permission; }))
            .padding(0.2);
        svg.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(x))
            .selectAll("text")
            .attr("transform", "translate(-10,0)rotate(-45)")
            .style("text-anchor", "end");

        // Add Y axis
        var y = d3.scaleLinear()
            .domain([0, 20])
            .range([height, 0]);
        svg.append("g")
            .call(d3.axisLeft(y));

        // Bars
        svg.selectAll("mybar")
            .data(data)
            .enter()
            .append("rect")
            .attr("x", function (d) { return x(d.permission); })
            .attr("y", function (d) { return y(d.count); })
            .attr("width", x.bandwidth())
            .attr("height", function (d) { return height - y(d.count); })
            .attr("fill", "purple")
    })
}

showCategoryChart();
showUserPermissionCharts();