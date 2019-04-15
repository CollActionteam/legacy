export default function registerGlobal (globalName: string, func): void {
  window["CollAction"] = window["CollAction"] || {};
  if(window["CollAction"][globalName]) {
    throw new Error(`Already Registered ${globalName}`);
  }

  window["CollAction"][globalName] = func;
}
