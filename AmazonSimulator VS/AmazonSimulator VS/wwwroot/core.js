function parseCommand(input = "") {
    return JSON.parse(input);
}

var Socket;

window.onload = function () {
    var camera, renderer;
    var cameraControls;

    function init() {
        camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 1, 1500);
        cameraControls = new THREE.OrbitControls(camera);
        camera.position.z = 15;
        camera.position.y = 20;
        camera.position.x = 50;
        cameraControls.update();

        renderer = new THREE.WebGLRenderer({ antialias: true });
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.setSize(window.innerWidth, window.innerHeight + 5);
        renderer.shadowMap.enabled = true;
        renderer.shadowMap.type = THREE.PCFShadowMap;
        document.body.appendChild(renderer.domElement);

        window.addEventListener('resize', onWindowResize, false);
    }

    function onWindowResize() {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    function animate() {
        requestAnimationFrame(animate);
        cameraControls.update();
        renderer.render(mainScene.scene, camera);
    }

    Socket = new WebSocket("ws://" + window.location.hostname + ":" + window.location.port + "/connect_client");
    Socket.onmessage = function (event) {
        mainScene.updateScene(parseCommand(event.data));
    };

    init();
    var mainScene = new MainScene();
    animate();
}