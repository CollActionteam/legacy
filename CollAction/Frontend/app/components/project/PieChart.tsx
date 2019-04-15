import * as React from "react";
import * as ReactDOM from "react-dom";
import renderComponentIf from "../global/renderComponentIf";

interface IPieChartProps {
    percentComplete: number;
}

class PieChart extends React.Component<IPieChartProps, {}>
{
    constructor(props) {
        super(props);
    }

    render() {
        return <canvas ref="canvas" />;
    }

    componentDidMount() {
        this.updateCanvas();
    }

    componentDidUpdate() {
        this.updateCanvas();
    }

    updateCanvas() {
        const canvas = this.refs.canvas as HTMLCanvasElement;
        const ctx = canvas.getContext("2d");
        const data = [this.props.percentComplete, 100 - this.props.percentComplete ]; // If you add more data values make sure you add more colors
        const myColor = ['#9bd8bc', '#23d844' ]; // Colors of each slice

        ctx.canvas.width = 100;
        ctx.canvas.height = 100;

        var lastend = 4.71239;
        for (var i = 0; i < data.length; i++) {
            ctx.fillStyle = myColor[i];
            ctx.beginPath();
            ctx.moveTo(canvas.width / 2, canvas.height / 2);
            // Arc Parameters: x, y, radius, startingAngle (radians), endingAngle (radians), antiClockwise (boolean)
            ctx.arc(canvas.width / 2, canvas.height / 2, canvas.height / 2, lastend, lastend + (Math.PI * 2 * (data[i] / 100)), false);
            ctx.lineTo(canvas.width / 2, canvas.height / 2);
            ctx.fill();
            lastend += Math.PI * 2 * (data[i] / 100);
        }
    }
};

renderComponentIf(
    <PieChart percentComplete={parseInt(document.getElementById("project-pie-chart") && document.getElementById("project-pie-chart").dataset.percentComplete || "0")} />,
    document.getElementById("project-pie-chart")
);