import "whatwg-fetch";
import * as Promise from "bluebird";

// Configure
Promise.config({
    longStackTraces: true,
    warnings: true // note, run node with --trace-warnings to see full stack traces for warnings
});
